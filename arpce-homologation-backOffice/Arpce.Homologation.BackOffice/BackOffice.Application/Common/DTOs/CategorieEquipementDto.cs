namespace BackOffice.Application.Common.DTOs;

public class CategorieEquipementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string TypeEquipement { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty;
    public decimal? TarifEtude { get; set; }
    public decimal? TarifHomologation { get; set; }
    public byte? TarifHomologationParLot { get; set; }
    public int? TarifHomologationQuantiteParLot { get; set; }
    public decimal? TarifControle { get; set; }
    public string? FormuleHomologation { get; set; }
    public int? QuantiteReference { get; set; }
    public string? Remarques { get; set; }
}