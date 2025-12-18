using BackOffice.Domain.Common;

namespace BackOffice.Domain.Entities;

public class Attestation : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid IdDemande { get; set; }
    public Demande Demande { get; set; } = null!;

    public long DateDelivrance { get; set; }
    public long DateExpiration { get; set; }
    public byte[] Donnees { get; set; } = Array.Empty<byte>();
    public string Extension { get; set; } = "pdf";
    public int NumeroSequentiel { get; set; }
}