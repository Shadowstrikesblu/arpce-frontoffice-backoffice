namespace FrontOffice.Domain.Entities;

public class Statut
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public string Libelle { get; set; }
}