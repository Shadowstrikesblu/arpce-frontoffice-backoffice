using System;

namespace FrontOffice.Application.Common.DTOs;

public class DevisDto
{
    public Guid Id { get; set; }
    public long Date { get; set; }
    public decimal MontantEtude { get; set; }
    public decimal? MontantHomologation { get; set; }
    public decimal? MontantControle { get; set; }
    public byte? PaiementOk { get; set; }
}