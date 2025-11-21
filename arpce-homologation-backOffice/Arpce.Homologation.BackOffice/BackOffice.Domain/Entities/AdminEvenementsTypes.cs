namespace BackOffice.Domain.Entities;
public class AdminEvenementsTypes
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string Libelle { get; set; } = string.Empty;
}