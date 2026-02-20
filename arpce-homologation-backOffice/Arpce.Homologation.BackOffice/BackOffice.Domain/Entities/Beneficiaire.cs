using BackOffice.Domain.Common;

namespace BackOffice.Domain.Entities;

public class Beneficiaire : AuditableEntity
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }

    // Relation 1:1 avec la demande (équipement)
    public Guid DemandeId { get; set; }
    public virtual Demande Demande { get; set; } = default!;
}