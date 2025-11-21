using BackOffice.Domain.Common; 
namespace BackOffice.Domain.Entities;
public class Proposition : AuditableEntity 
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
}