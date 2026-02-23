using MediatR;
using System;

namespace BackOffice.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Commande pour enregistrer un nouvel utilisateur du back-office (agent).
/// </summary>
public class RegisterAgentCommand : IRequest<AuthenticationResult>
{
    /// <summary>
    /// Le nom de compte (login) de l'agent. Doit être unique.
    /// </summary>
    public string Compte { get; set; } = string.Empty;

    /// <summary>
    /// L'adresse e-mail de l'agent (utilisée pour les notifications).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Le nom de famille de l'agent.
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Le(s) prénom(s) de l'agent.
    /// </summary>
    public string? Prenoms { get; set; }

    /// <summary>
    /// Le mot de passe initial de l'agent.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}