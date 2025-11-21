using FrontOffice.Domain.Common;
using FrontOffice.Domain.Entities;

public class Devis : AuditableEntity
{
   
    public Guid IdDemande { get; set; } 

    public DateTime Date { get; set; }
    public decimal MontantEtude { get; set; }
    public decimal? MontantHomologation { get; set; }
    public decimal? MontantControle { get; set; }
    public byte? PaiementOk { get; set; }
    public string? PaiementMobileId { get; set; }

    public virtual Demande Demande { get; set; } = default!; 
}