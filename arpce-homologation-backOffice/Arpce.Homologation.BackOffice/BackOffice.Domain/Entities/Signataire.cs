using BackOffice.Domain.Common;

namespace BackOffice.Domain.Entities;

public class Signataire : AuditableEntity
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenoms { get; set; } = string.Empty;
    public string Fonction { get; set; } = string.Empty;
    public string? SignatureImagePath { get; set; } 
    public bool IsActive { get; set; } = true;
    public Guid AdminId { get; set; }
}