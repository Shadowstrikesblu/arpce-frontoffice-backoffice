using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

public class AdminProfils : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Remarques { get; set; }


    public virtual ICollection<AdminUtilisateur> Utilisateurs { get; set; } = new List<AdminUtilisateur>();
    public virtual ICollection<AdminProfilsAcces> AdminProfilsAcces { get; set; } = new List<AdminProfilsAcces>();
    public virtual ICollection<AdminProfilsUtilisateursLDAP> AdminProfilsUtilisateursLDAP { get; set; } = new List<AdminProfilsUtilisateursLDAP>();
}