namespace FrontOffice.Domain.Entities;

public class AdminProfils
{
    public Guid Id { get; set; } 
    public string? Code { get; set; } 
    public string Libelle { get; set; } = string.Empty;
    public string? Remarques { get; set; } 
    public string? UtilisateurCreation { get; set; } 
    public DateTime? DateCreation { get; set; } 
    public string? UtilisateurModification { get; set; } 
    public DateTime? DateModification { get; set; } 

}