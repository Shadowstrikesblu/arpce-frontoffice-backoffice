// Fichier : BackOffice.Api/Controllers/DocumentsController.cs

using BackOffice.Application.Common.DTOs.Documents;
using BackOffice.Application.Features.Demandes.Queries.DownloadDocument;
using BackOffice.Application.Features.Documents.Queries.GetAttestationsList;
using BackOffice.Application.Features.Documents.Queries.GetFacturesList;
using BackOffice.Application.Features.Documents.Queries.GetPaiementsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BackOffice.Api.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly ISender _mediator;

    public DocumentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("factures")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DocumentListVm))]
    public async Task<IActionResult> GetFactures([FromQuery] GetFacturesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("paiements")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DocumentListVm))]
    public async Task<IActionResult> GetPaiements([FromQuery] GetPaiementsListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("attestations")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AttestationListVm))]
    public async Task<IActionResult> GetAttestations([FromQuery] GetAttestationsListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Pour récupérer les fichiers
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{type}/{id:guid}/download")]
    public async Task<IActionResult> DownloadDocument(string type, Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DownloadDocumentQuery(id, type));
            return File(result.FileContents, result.ContentType, result.FileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}