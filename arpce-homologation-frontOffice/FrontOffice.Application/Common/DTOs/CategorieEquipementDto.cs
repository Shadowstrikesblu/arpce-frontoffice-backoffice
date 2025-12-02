namespace FrontOffice.Application.Common.DTOs;
public class CategorieEquipementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public decimal? TarifEtude { get; set; }
    public decimal? TarifHomologation { get; set; }
}