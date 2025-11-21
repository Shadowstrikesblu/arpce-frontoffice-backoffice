namespace FrontOffice.Application.Features.Authentication.Queries.ConnectByToken;

/// <summary>
/// Représente le résultat d'une validation de session réussie via un token.
/// </summary>
public class ConnectByTokenResult
{
    /// <summary>
    /// Message de succès.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// L'identifiant de l'utilisateur connecté.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// L'adresse e-mail de l'utilisateur.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// La raison sociale de l'utilisateur.
    /// </summary>
    public string RaisonSociale { get; set; } = string.Empty;

    /// <summary>
    /// Le nom du contact de l'utilisateur.
    /// </summary>
    public string? ContactNom { get; set; }
}