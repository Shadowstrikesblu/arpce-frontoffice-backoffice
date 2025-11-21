using System;

namespace BackOffice.Application.Features.Categories.Queries.GetCategoriesList;

/// <summary>
/// DTO représentant une Catégorie d'Équipement dans une liste.
/// </summary>
public class CategorieEquipementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string TypeEquipement { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty; 

    public decimal? FraisEtude { get; set; }
    public decimal? FraisHomologation { get; set; }
    public decimal? FraisControle { get; set; }

    public string? FormuleHomologation { get; set; }
    public int? QuantiteReference { get; set; }

    public string? Remarques { get; set; }
}