namespace FrontOffice.Application.Common.DTOs;
public class CategorieEquipementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public decimal? FraisEtude { get; set; }
    public decimal? FraisHomologation { get; set; }
    public decimal? FraisControle { get; set; }
}