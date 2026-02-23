using BackOffice.Application.Common.DTOs;
using System.Collections.Generic;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// ViewModel pour la réponse de la liste paginée des dossiers du Back Office.
/// </summary>
public class DossiersListVm
{
    public int Page { get; set; }
    public int PageTaille { get; set; }
    public int TotalPage { get; set; }
    public List<DossierListItemDto> Dossiers { get; set; } = new List<DossierListItemDto>();
}