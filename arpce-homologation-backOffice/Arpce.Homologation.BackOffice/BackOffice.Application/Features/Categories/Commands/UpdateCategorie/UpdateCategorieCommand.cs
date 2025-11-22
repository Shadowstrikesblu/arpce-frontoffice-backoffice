using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.UpdateCategorie;

public class UpdateCategorieCommand : IRequest<bool>
{
    public Guid Id { get; set; } 
    public string? Code { get; set; }
    public string? Libelle { get; set; }
    public string? TypeEquipement { get; set; }
    public string? TypeClient { get; set; }
    public decimal? FraisEtude { get; set; }
    public decimal? FraisHomologation { get; set; }
    public decimal? FraisControle { get; set; }
    public string? FormuleHomologation { get; set; }
    public int? QuantiteReference { get; set; }
    public string? Remarques { get; set; }
}