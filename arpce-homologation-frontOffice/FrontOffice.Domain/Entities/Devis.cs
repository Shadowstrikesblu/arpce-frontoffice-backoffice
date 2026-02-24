using FrontOffice.Domain.Common; 
using System;

namespace FrontOffice.Domain.Entities;

public class Devis : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid IdDossier { get; set; }
    public Guid? IdDemande { get; set; }
    public decimal MontantPenalite { get; set; } = 0;
    public long Date { get; set; }
    public decimal MontantEtude { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal? MontantHomologation { get; set; }
    public decimal? MontantControle { get; set; }
    public byte? PaiementOk { get; set; }
    public string? PaiementMobileId { get; set; }

    public virtual Dossier Dossier { get; set; } = default!;
    public virtual Demande? Demande { get; set; }
}