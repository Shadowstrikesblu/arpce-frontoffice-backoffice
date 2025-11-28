using BackOffice.Domain.Common;

namespace BackOffice.Domain.Entities;

public class AdminUtilisateur : AuditableEntity
{
    public string Compte { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Prenoms { get; set; }
    public string? MotPasse { get; set; }
    public bool ChangementMotPasse { get; set; }
    public bool Desactive { get; set; }
    public DateTime? DerniereConnexion { get; set; }
    public string? Remarques { get; set; }

    public Guid? IdProfil { get; set; } 
    public virtual AdminProfils? Profil { get; set; }
}