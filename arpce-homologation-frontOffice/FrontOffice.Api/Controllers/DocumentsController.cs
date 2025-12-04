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
    /// Télécharge un document (dossier ou demande) par son ID.
    /// </summary>
    /// <param name="type">Le type de document : 'dossier' ou 'demande'.</param>
    /// <param name="id">L'ID du document.</param>
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