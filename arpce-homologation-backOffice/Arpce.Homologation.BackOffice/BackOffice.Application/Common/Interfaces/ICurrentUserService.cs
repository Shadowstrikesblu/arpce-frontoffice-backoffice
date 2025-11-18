namespace BackOffice.Application.Common.Interfaces;

/// <summary>
/// Définit un contrat pour un service qui fournit les informations
/// de l'utilisateur (agent) actuellement authentifié.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtient l'identifiant unique (Guid) de l'utilisateur courant.
    /// Retourne null si l'utilisateur n'est pas authentifié.
    /// </summary>
    Guid? UserId { get; }

    // On pourrait ajouter d'autres propriétés utiles ici à l'avenir,
    // comme le profil, le nom d'utilisateur, etc.
}