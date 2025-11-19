namespace FrontOffice.Domain.Entities;

public class Proposition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public string Libelle { get; set; }
}