namespace FrontOffice.Domain.Entities;

public class AdminAccess
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Application { get; set; } = string.Empty;
    public string Groupe { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Page { get; set; }
    public string Type { get; set; } = string.Empty;
    public byte? Inactif { get; set; }
    public byte? Ajouter { get; set; }
    public byte? Valider { get; set; }
    public byte? Supprimer { get; set; }
    public byte? Imprimer { get; set; }
    public string Code { get; set; } = string.Empty;
}