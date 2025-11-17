namespace FrontOffice.Domain.Entities;

public class AdminProfilsAcces
{
    public Guid IdProfil { get; set; } 
    public Guid IdAccess { get; set; } 
    public byte? Ajouter { get; set; } 
    public byte? Valider { get; set; } 
    public byte? Supprimer { get; set; } 
    public byte? Imprimer { get; set; } 

    public AdminProfils? Profil { get; set; }
    public AdminAccess? Access { get; set; }
}