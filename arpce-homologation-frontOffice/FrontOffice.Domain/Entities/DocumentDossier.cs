namespace FrontOffice.Domain.Entities;

public class DocumentDossier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdDossier { get; set; }
    public string? Nom { get; set; }
    public byte? Type { get; set; }
    public byte[]? Donnees { get; set; }
    public string Extension { get; set; }
    public string? FilePath { get; set; }

    public virtual Dossier Dossier { get; set; }
}