using BackOffice.Domain.Common; 
namespace BackOffice.Domain.Entities;
public class MotifRejet : AuditableEntity 
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Remarques { get; set; }
}