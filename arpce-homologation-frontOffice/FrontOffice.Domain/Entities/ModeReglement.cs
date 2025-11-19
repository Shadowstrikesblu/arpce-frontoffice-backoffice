namespace FrontOffice.Domain.Entities;

public class ModeReglement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public string Libelle { get; set; }
    public byte MobileBanking { get; set; }
    public string? Remarques { get; set; }
    public string? UtilisateurCreation { get; set; }
    public DateTime? DateCreation { get; set; }
    public string? UtilisateurModification { get; set; }
    public DateTime? DateModification { get; set; }
}