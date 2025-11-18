namespace BackOffice.Application.Features.Authentication;

/// <summary>
/// Représente le résultat d'une opération d'authentification réussie (inscription ou connexion).
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// Le token JWT généré pour la session de l'utilisateur.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Un message de succès décrivant l'opération.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}