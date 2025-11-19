using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Features.Clients.Commands.UpdateClientContact;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Api.Controllers;

[ApiController]
[Route("api/clients")] 
[Authorize] 
public class ClientsController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur.
    /// </summary>
    /// <param name="mediator">L'instance de MediatR.</param>
    public ClientsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Met à jour les informations de contact du client spécifié.
    /// L'utilisateur authentifié ne peut mettre à jour que son propre profil.
    /// </summary>
    /// <param name="clientId">L'identifiant du client à mettre à jour (doit correspondre à l'utilisateur connecté).</param>
    /// <param name="command">Les nouvelles informations de contact. Les champs non fournis (null) ne seront pas modifiés.</param>
    /// <returns>Les informations de contact mises à jour.</returns>
    [HttpPatch("{clientId:guid}/contact")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientContactDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    public async Task<IActionResult> UpdateClientContact(Guid clientId, [FromBody] UpdateClientContactCommand command)
    {
        // Assigne l'ID de la route à la commande pour que le handler puisse l'utiliser pour la validation.
        command.ClientId = clientId;

        var result = await _mediator.Send(command);

        return Ok(result);
    }
}