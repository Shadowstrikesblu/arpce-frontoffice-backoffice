namespace FrontOffice.Domain.Entities;

public class CategorieEquipement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public string Libelle { get; set; }
    public decimal? TarifEtude { get; set; }
    public decimal? TarifHomologation { get; set; }
    public byte? TarifHomologationParLot { get; set; }
    public int? TarifHomologationQuantiteParLot { get; set; }
    public decimal? TarifControle { get; set; }
    public string? Remarques { get; set; }
    public string? UtilisateurCreation { get; set; }
    public long? DateCreation { get; set; }
    public string? UtilisateurModification { get; set; }
    public long? DateModification { get; set; }
}