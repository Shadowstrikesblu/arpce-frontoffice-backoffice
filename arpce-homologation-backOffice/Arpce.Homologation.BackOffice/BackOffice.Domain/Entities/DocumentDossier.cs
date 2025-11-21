using BackOffice.Domain.Common; 
namespace BackOffice.Domain.Entities;
public class DocumentDossier : AuditableEntity 
{
    public Guid IdDossier { get; set; }
    public string? Nom { get; set; }
    public byte? Type { get; set; } 
    public byte[]? Donnees { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string? FilePath { get; set; } 
    public virtual Dossier Dossier { get; set; } = default!;
}