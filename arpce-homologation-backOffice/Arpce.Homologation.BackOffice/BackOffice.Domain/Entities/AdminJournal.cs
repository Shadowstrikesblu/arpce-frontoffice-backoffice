namespace BackOffice.Domain.Entities;
public class AdminJournal
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public Guid IdEvenementType { get; set; }
    public string Application { get; set; } = string.Empty;
    public string AdresseIP { get; set; } = string.Empty;
    public string Utilisateur { get; set; } = string.Empty;
    public DateTime DateEvenement { get; set; }
    public string Page { get; set; } = string.Empty;
    public string? Libelle { get; set; }
    public virtual AdminEvenementsTypes? EvenementType { get; set; }
}