using BackOffice.Application.Common.DTOs;
using MediatR;

namespace BackOffice.Application.Features.Categories.Queries.GetCategoriesList;

/// <summary>
/// Requête pour obtenir la liste des catégories, potentiellement filtrée par type d'équipement.
/// </summary>
public class GetCategoriesListQuery : IRequest<List<CategorieEquipementDto>>
{
    /// <summary>
    /// Le type d'équipement par lequel filtrer la liste. Si null, toutes les catégories sont retournées.
    /// </summary>
    public string? TypeEquipement { get; set; } 
}