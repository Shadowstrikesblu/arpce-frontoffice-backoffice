namespace BackOffice.Application.Features.Authentication.Queries.CheckToken;

public class AdminAccessDto
{
    public Guid Id { get; set; }
    public string Application { get; set; } = string.Empty;
    public string Groupe { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Page { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Inactif { get; set; } 
    public bool Ajouter { get; set; }
    public bool Valider { get; set; }
    public bool Supprimer { get; set; }
    public bool Imprimer { get; set; }
}