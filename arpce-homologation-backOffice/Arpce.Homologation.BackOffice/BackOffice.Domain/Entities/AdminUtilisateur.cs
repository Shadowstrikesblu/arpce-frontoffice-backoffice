namespace BackOffice.Domain.Entities;

public class AdminUtilisateur
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? IdProfil { get; set; }
    public Guid IdUtilisateurType { get; set; }
    public string Compte { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Prenoms { get; set; }
    public string? MotPasse { get; set; }
    public bool ChangementMotPasse { get; set; }
    public bool Desactive { get; set; }
    public string? Remarques { get; set; }
    public DateTime? DerniereConnexion { get; set; }
    public string? UtilisateurCreation { get; set; }
    public DateTime? DateCreation { get; set; }
    public string? UtilisateurModification { get; set; }
    public DateTime? DateModification { get; set; }

    public virtual AdminProfils? Profil { get; set; }

    public virtual AdminUtilisateurTypes? UtilisateurType { get; set; }
}