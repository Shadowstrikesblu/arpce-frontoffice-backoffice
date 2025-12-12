using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

public class CategorieEquipement : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    public string TypeEquipement { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty;
    public string? FormuleHomologation { get; set; }
    public int? QuantiteReference { get; set; }
    public decimal? FraisEtude { get; set; }
    public decimal? FraisHomologation { get; set; }
    public byte? FraisHomologationParLot { get; set; }
    public int? FraisHomologationQuantiteParLot { get; set; }
    public decimal? FraisControle { get; set; }
    public string? Remarques { get; set; }
}