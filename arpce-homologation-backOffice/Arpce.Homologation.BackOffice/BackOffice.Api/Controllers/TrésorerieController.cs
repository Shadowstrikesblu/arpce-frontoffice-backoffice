using BackOffice.Application.Features.Dossiers.Commands.UploadReceipt;
using BackOffice.Application.Features.Paiements.Commands.EnregistrerPaiementCaisse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TresorerieController : ControllerBase
{
    private readonly ISender _mediator;
    public TresorerieController(ISender mediator) => _mediator = mediator;

    /// <summary>
    /// Enregistre un paiement physique et retourne le lien du reçu généré.
    /// </summary>
    [HttpPost("paiement-caisse")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaiementCaisseResult))]
    public async Task<IActionResult> EnregistrerPaiement([FromBody] EnregistrerPaiementCaisseCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Upload la preuve de paiement signée (Reçu scanné) pour un dossier.
    /// </summary>
    [HttpPost("{dossierId:guid}/upload-recu-signe")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadReceipt(Guid dossierId, IFormFile recuFile)
    {
        var result = await _mediator.Send(new UploadReceiptCommand
        {
            DossierId = dossierId,
            RecuFile = recuFile
        });

        return Ok(new { success = result, message = "Le reçu signé a été ajouté aux documents du dossier." });
    }
}