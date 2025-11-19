namespace FrontOffice.Application.Common.Interfaces;

/// <summary>
/// Définit un service pour récupérer les informations de l'utilisateur courant.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtient l'identifiant de l'utilisateur courant.
    /// Peut être null si l'utilisateur n'est pas authentifié.
    /// </summary>
    Guid? UserId { get; }
}