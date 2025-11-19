using BackOffice.Application.Features.Authentication; 
using BackOffice.Application.Features.Authentication.Commands.Register; 
using BackOffice.Application.Features.Authentication.Queries.Login; 
using MediatR; 
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

/// <summary>
/// Contrôleur gérant les opérations d'authentification pour les agents du Back Office.
/// </summary>
[ApiController]
[Route("api/auth")] 
public class AuthenticationController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur.
    /// </summary>
    /// <param name="mediator">L'instance de MediatR pour envoyer les commandes et les requêtes.</param>
    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Enregistre un nouvel agent du Back Office.
    /// Ce endpoint sera normalement sécurisé pour n'être accessible qu'aux administrateurs.
    /// </summary>
    /// <param name="command">Les informations nécessaires pour créer le compte de l'agent.</param>
    /// <returns>Un token JWT et un message de succès.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    public async Task<IActionResult> RegisterAgent([FromBody] RegisterAgentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Permet à un agent du Back Office de se connecter et d'obtenir un token JWT.
    /// </summary>
    /// <param name="query">Les informations d'identification de l'agent (compte et mot de passe).</param>
    /// <returns>Un token JWT et un message de succès.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResult))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
    public async Task<IActionResult> LoginAgent([FromBody] LoginAgentQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}