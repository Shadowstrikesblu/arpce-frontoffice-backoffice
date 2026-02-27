using MediatR;
using System;

namespace BackOffice.Application.Features.Paiements.Commands.EnregistrerPaiementCaisse;

public record PaiementCaisseResult(Guid DocumentId, string FilePath);

public class EnregistrerPaiementCaisseCommand : IRequest<PaiementCaisseResult>
{
    public string NumeroDossier { get; set; } = string.Empty;
    public decimal MontantEncaisse { get; set; }
    public string? NumeroQuittance { get; set; }
    public string ModePaiement { get; set; } = "Espèces";
}