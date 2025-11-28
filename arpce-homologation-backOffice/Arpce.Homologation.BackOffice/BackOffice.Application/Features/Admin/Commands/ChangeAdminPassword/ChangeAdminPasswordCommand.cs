using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.ChangeAdminPassword;

/// <summary>
/// Commande pour modifier le mot de passe d'un administrateur.
/// </summary>
public class ChangeAdminPasswordCommand : IRequest<bool>
{
    /// <summary>
    /// L'identifiant de l'utilisateur administrateur dont le mot de passe doit être changé.
    /// </summary>
    public Guid UtilisateurId { get; set; }

    /// <summary>
    /// Le nouveau mot de passe (en clair).
    /// </summary>
    public string MotDePasse { get; set; } = string.Empty;
}