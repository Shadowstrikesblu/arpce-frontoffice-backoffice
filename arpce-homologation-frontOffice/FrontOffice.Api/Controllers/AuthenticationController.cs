using FrontOffice.Application.Features.Authentication.Commands.Register;
using FrontOffice.Application.Features.Authentication.Queries.Login; // Ajout du using
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    // Méthode d'enregistrement
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterClientCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Méthode de connexion
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginClientQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}