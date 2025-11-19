namespace FrontOffice.Domain.Entities;

public class Devis
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdDossier { get; set; }
    public DateTime Date { get; set; }
    public decimal MontantEtude { get; set; }
    public decimal? MontantHomologation { get; set; }
    public decimal? MontantControle { get; set; }
    public byte? PaiementOk { get; set; }
    public string? PaiementMobileId { get; set; }

    public virtual Dossier Dossier { get; set; }
}