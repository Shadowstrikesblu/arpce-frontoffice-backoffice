namespace FrontOffice.Application.Common.DTOs;

public class DevisDto
{
    public Guid Id { get; set; }
    public long Date { get; set; }
    public decimal MontantEtude { get; set; }
    public decimal? MontantHomologation { get; set; }
    public decimal? MontantControle { get; set; }
    public decimal MontantPenalite { get; set; }
    public decimal MontantTotal { get; set; }
    public byte? PaiementOk { get; set; }
    public string? FilePath { get; set; }
}