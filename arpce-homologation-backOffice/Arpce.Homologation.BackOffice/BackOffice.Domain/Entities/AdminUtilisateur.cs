namespace BackOffice.Domain.Entities;

public class AdminUtilisateur
{
    public Guid Id { get; set; } = Guid.NewGuid();
    // public Guid? IdProfil { get; set; } 
    public string Compte { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Prenoms { get; set; }
    public string? MotPasse { get; set; }
    public bool ChangementMotPasse { get; set; }
    public bool Desactive { get; set; }
    public DateTime? DerniereConnexion { get; set; }

    // public virtual AdminProfil? Profil { get; set; } 
}