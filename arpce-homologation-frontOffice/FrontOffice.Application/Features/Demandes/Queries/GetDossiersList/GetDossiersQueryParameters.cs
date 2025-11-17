namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Contient tous les paramètres de requête pour la recherche, le filtrage,
/// le tri et la pagination de la liste des dossiers.
/// </summary>
public class GetDossiersQueryParameters
{
    /// <summary>
    /// Le numéro de la page à retourner. La valeur par défaut est 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Le nombre de dossiers à retourner par page. La valeur par défaut est 10.
    /// </summary>
    public int TaillePage { get; set; } = 10;

    /// <summary>
    /// Le terme de recherche utilisé pour filtrer les dossiers.
    /// La recherche s'appliquera sur le numéro, le libellé, et potentiellement d'autres champs.
    /// </summary>
    public string? Recherche { get; set; }

    /// <summary>
    /// Le champ sur lequel les résultats doivent être triés.
    /// Valeurs possibles : "date_creation", "date-update", "libelle".
    /// </summary>
    public string? TrierPar { get; set; }

    /// <summary>
    /// L'ordre du tri. Valeurs possibles : "asc" (ascendant) ou "desc" (descendant).
    /// </summary>
    public string? Ordre { get; set; }
}