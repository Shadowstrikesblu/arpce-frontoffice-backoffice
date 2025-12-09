namespace BackOffice.Domain.Entities;
public class AdminConnexions
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string Utilisateur { get; set; } = string.Empty;
    public long DateConnexion { get; set; }
    public string? Ip { get; set; }
}