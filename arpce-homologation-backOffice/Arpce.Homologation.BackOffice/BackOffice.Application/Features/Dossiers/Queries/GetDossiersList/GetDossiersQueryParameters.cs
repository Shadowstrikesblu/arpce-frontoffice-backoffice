namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Contient tous les paramètres pour la recherche, le filtrage, le tri et la pagination de la liste des dossiers du Back Office.
/// </summary>
public class GetDossiersQueryParameters
{
    public int Page { get; set; } = 1;
    public int TaillePage { get; set; } = 10;
    public string? Recherche { get; set; }
    public string? Status { get; set; } 
    public string? TrierPar { get; set; } 
    public string? Ordre { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
}