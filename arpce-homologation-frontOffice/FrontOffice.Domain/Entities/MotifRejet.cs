using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

public class MotifRejet : AuditableEntity
{
    public string Code { get; set; }
    public string Libelle { get; set; }
    public string? Remarques { get; set; }
    public string? UtilisateurCreation { get; set; }
    public DateTime? DateCreation { get; set; }
    public string? UtilisateurModification { get; set; }
    public DateTime? DateModification { get; set; }
}