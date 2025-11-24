using BackOffice.Domain.Common;

namespace BackOffice.Domain.Entities;

/// <summary>
/// Représente un profil ou un rôle d'utilisateur dans le back-office.
/// Mappé sur la table 'adminProfils'.
/// </summary>
public class AdminProfils : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Remarques { get; set; }


    // Propriété de navigation pour lister les utilisateurs ayant ce profil
    public virtual ICollection<AdminUtilisateur> Utilisateurs { get; set; } = new List<AdminUtilisateur>();
}