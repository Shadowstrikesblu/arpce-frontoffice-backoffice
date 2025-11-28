using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.AssignProfilToAdmin;

/// <summary>
/// Commande pour attribuer un profil spécifique à un utilisateur administrateur.
/// </summary>
public class AssignProfilToAdminCommand : IRequest<bool>
{
    /// <summary>
    /// L'identifiant de l'utilisateur administrateur à mettre à jour.
    /// </summary>
    public Guid UtilisateurId { get; set; }

    /// <summary>
    /// L'identifiant du profil à lui attribuer.
    /// </summary>
    public Guid IdProfil { get; set; }
}