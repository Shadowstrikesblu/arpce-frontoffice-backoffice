using System.Collections.Generic;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// ViewModel pour la réponse de la liste paginée des dossiers.
/// Contient les données des dossiers ainsi que les informations de pagination.
/// </summary>
public class DossiersListVm
{
    /// <summary>
    /// Le numéro de la page actuelle.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Le nombre total de pages disponibles.
    /// </summary>
    public int TotalPage { get; set; }

    /// <summary>
    /// Le terme de recherche qui a été appliqué.
    /// </summary>
    public string? Recherche { get; set; }

    /// <summary>
    /// La liste des dossiers pour la page actuelle.
    /// </summary>
    public List<DossierListItemDto> Dossiers { get; set; } = new List<DossierListItemDto>();
}