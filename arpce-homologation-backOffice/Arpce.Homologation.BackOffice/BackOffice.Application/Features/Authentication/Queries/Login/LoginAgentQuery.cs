using MediatR;

namespace BackOffice.Application.Features.Authentication.Queries.Login;

/// <summary>
/// Requête pour authentifier un utilisateur du back-office (agent) et obtenir un token JWT.
/// </summary>
public class LoginAgentQuery : IRequest<AuthenticationResult>
{
    /// <summary>
    /// Le nom de compte (login) de l'agent.
    /// </summary>
    public string Compte { get; set; } = string.Empty;

    /// <summary>
    /// Le mot de passe de l'agent.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}