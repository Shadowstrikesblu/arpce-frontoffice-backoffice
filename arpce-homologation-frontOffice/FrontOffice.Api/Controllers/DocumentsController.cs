using FrontOffice.Application.Features.Documents.Queries.DownloadCertificat; 
using FrontOffice.Application.Features.Documents.Queries.DownloadDocument;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Api.Controllers;

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

    /// <summary>
    /// Télécharge un document générique (lié à un dossier ou une demande) par son ID.
    /// </summary>
    /// <param name="type">Le type de document : 'dossier' ou 'demande'.</param>
    /// <param name="id">L'ID du document.</param>
    [HttpGet("{type}/{id:guid}/download")]
    public async Task<IActionResult> DownloadDocument(string type, Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DownloadDocumentQuery(id, type));
            // La méthode File() de ASP.NET Core construit la réponse HTTP pour le téléchargement
            return File(result.FileContents, result.ContentType, result.FileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Télécharge un certificat (attestation) spécifique par son ID.
    /// </summary>
    /// <param name="id">L'ID de l'attestation.</param>
    [HttpGet("certificat/{id:guid}/download")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadCertificat(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DownloadCertificatQuery(id));
            return File(result.FileContents, result.ContentType, result.FileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}