using BackOffice.Domain.Common; 
namespace BackOffice.Domain.Entities;
public class DocumentDemande : AuditableEntity 
{
    public Guid IdDemande { get; set; }
    public string? Nom { get; set; }
    public string? Libelle { get; set; }
    public byte[]? Donnees { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string? FilePath { get; set; } 
    public virtual Demande Demande { get; set; } = default!;
}