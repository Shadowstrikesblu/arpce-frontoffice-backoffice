namespace FrontOffice.Domain.Entities;

public class Attestation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdDemande { get; set; }
    public DateTime DateDelivrance { get; set; }
    public DateTime DateExpiration { get; set; }
    public byte[]? Donnees { get; set; }
    public string Extension { get; set; }

    public virtual Demande Demande { get; set; }
}