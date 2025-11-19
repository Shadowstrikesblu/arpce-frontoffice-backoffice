namespace FrontOffice.Domain.Entities;

public class AdminUtilisateurs
{
    public Guid Id { get; set; } 
    public Guid IdUtilisateurType { get; set; } 
    public Guid? IdProfil { get; set; } 
    public string Compte { get; set; } = string.Empty; 
    public string Nom { get; set; } = string.Empty; 
    public string? Prenoms { get; set; } 
    public string? MotPasse { get; set; } 
    public byte? ChangementMotPasse { get; set; } 
    public byte? Desactive { get; set; } 
    public string? Remarques { get; set; } 
    public DateTime? DerniereConnexion { get; set; } 
    public string? UtilisateurCreation { get; set; } 
    public DateTime? DateCreation { get; set; } 
    public string? UtilisateurModification { get; set; } 
    public DateTime? DateModification { get; set; } 

    public AdminUtilisateurTypes? UtilisateurType { get; set; }
    public AdminProfils? Profil { get; set; }
    
}