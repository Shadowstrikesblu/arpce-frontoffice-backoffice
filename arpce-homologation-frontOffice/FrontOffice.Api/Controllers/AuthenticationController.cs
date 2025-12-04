using FrontOffice.Application.Features.Authentication;
using FrontOffice.Application.Features.Authentication.Commands.ChangePassword;
using FrontOffice.Application.Features.Authentication.Commands.ConfirmAccount;
using FrontOffice.Application.Features.Authentication.Commands.ForgotPassword;
using FrontOffice.Application.Features.Authentication.Commands.Register;
using FrontOffice.Application.Features.Authentication.Commands.ResendOtp;
using FrontOffice.Application.Features.Authentication.Commands.ResetPassword;
using FrontOffice.Application.Features.Authentication.Queries.ConnectByToken;
using FrontOffice.Application.Features.Authentication.Queries.Login; // Ajout du using
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("confirm-account")]
    [Authorize] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResult))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)] 
    public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Valide une session existante à l'aide d'un token de connexion valide.
    /// </summary>
    /// <returns>Les informations de base de l'utilisateur si le token est valide.</returns>

    [HttpPost("connect-by-token")]
    [Authorize] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConnectByTokenResult))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
    public async Task<IActionResult> ConnectByToken()
    {
        // On crée la query, même si elle est vide, pour déclencher le handler.
        var query = new ConnectByTokenQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpPatch("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    [HttpPost("resend-otp")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> ResendOtp()
    {
        var result = await _mediator.Send(new ResendOtpCommand());
        return Ok(new { ok = result });
    }


    /// <summary>
    /// Déclenche le processus de réinitialisation de mot de passe pour un utilisateur.
    /// Un email avec un code OTP est envoyé si le compte existe.
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResult))]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        // La réponse est toujours 200 OK pour ne pas divulguer si un e-mail existe.
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Finalise la réinitialisation du mot de passe avec le code OTP et le nouveau mot de passe.
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)] // Pour un code invalide/expiré
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

}