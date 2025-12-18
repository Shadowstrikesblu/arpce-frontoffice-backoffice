using FrontOffice.Application.Features.Paiements.Commands.HandleMomoWebhook;
using FrontOffice.Application.Features.Paiements.Commands.PayByMomo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/paiements")]
[Authorize]
public class PaiementsController : ControllerBase
{
    private readonly ISender _mediator;
    public PaiementsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("dossier/{dossierId:guid}/momo")]
    public async Task<IActionResult> PayByMomo(Guid dossierId, [FromBody] PayByProviderCommand command)
    {
        command.DossierId = dossierId;
        var result = await _mediator.Send(command);
        return Ok(new { message = "Demande de paiement envoyée. Veuillez valider sur votre téléphone.", ok = result });
    }

    /// <summary>
    /// Momo webhook
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPost("momo-webhook")]
    [AllowAnonymous] // Le webhook doit être public
    public async Task<IActionResult> MomoWebhook([FromBody] MomoWebhookPayload payload)
    {
        // On envoie la charge utile au handler pour traitement asynchrone
        await _mediator.Send(new HandleMomoWebhookCommand(payload));

        // On retourne 200 OK immédiatement à l'API MTN, sans attendre le traitement
        return Ok();
    }
}

