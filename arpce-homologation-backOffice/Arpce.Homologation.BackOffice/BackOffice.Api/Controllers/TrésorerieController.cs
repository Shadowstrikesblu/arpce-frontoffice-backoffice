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
    /// Enregistre un paiement physique et génère le reçu PDF ARPCE.
    /// </summary>
    [HttpPost("paiement-caisse")]
    public async Task<IActionResult> EnregistrerPaiement([FromBody] EnregistrerPaiementCaisseCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = result });
    }
}