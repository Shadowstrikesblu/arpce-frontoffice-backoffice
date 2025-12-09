namespace FrontOffice.Domain.Common;

public abstract class AuditableEntity
{
    public Guid Id { get; set; }
    public string? UtilisateurCreation { get; set; }
    public long? DateCreation { get; set; } 
    public string? UtilisateurModification { get; set; }
    public long? DateModification { get; set; } // <-- MODIFIÉ
}