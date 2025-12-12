namespace FrontOffice.Domain.Entities;

public class Attestation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdDemande { get; set; }
    public long DateDelivrance { get; set; }
    public long DateExpiration { get; set; }
    public byte[]? Donnees { get; set; }
    public string Extension { get; set; } = string.Empty;
    public virtual Demande Demande { get; set; } = default!;
}