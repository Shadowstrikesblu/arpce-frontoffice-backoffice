using BackOffice.Application.Common.DTOs;
using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.CreateCategorie;

public class CreateCategorieCommand : IRequest<CategorieEquipementDto>
{
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