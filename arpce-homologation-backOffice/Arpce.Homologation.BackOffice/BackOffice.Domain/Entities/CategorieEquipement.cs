using BackOffice.Domain.Common; 
namespace BackOffice.Domain.Entities;
public class CategorieEquipement : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    public string TypeEquipement { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty; 
    public string? FormuleHomologation { get; set; }
    public int? QuantiteReference { get; set; }
    public decimal? TarifEtude { get; set; }
    public decimal? TarifHomologation { get; set; }
    public byte? TarifHomologationParLot { get; set; }
    public int? TarifHomologationQuantiteParLot { get; set; }
    public decimal? TarifControle { get; set; }
    public string? Remarques { get; set; }
}