namespace FrontOffice.Domain.Entities;

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
    public long? DerniereConnexion { get; set; }
    public string? UtilisateurCreation { get; set; }
    public long? DateCreation { get; set; }
    public string? UtilisateurModification { get; set; }
    public long? DateModification { get; set; }

    public virtual AdminProfils? Profil { get; set; }

    public virtual AdminUtilisateurTypes? UtilisateurType { get; set; }
}