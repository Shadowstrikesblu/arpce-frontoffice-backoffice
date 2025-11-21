namespace BackOffice.Domain.Entities;
public class AdminReporting
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string? Application { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public byte? Inactif { get; set; }
}