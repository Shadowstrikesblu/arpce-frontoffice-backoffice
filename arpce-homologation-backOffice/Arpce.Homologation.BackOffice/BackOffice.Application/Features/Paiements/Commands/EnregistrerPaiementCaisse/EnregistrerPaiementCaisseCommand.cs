using MediatR;
using System;

namespace BackOffice.Application.Features.Paiements.Commands.EnregistrerPaiementCaisse;

public class EnregistrerPaiementCaisseCommand : IRequest<bool>
{
    /// <summary>
    /// Numéro du dossier (ex: Hom-26-0001)
    /// </summary>
    public string NumeroDossier { get; set; } = string.Empty;

    /// <summary>
    /// Montant réellement perçu à la caisse
    /// </summary>
    public decimal MontantEncaisse { get; set; }

    /// <summary>
    /// Numéro de la quittance papier ou référence interne
    /// </summary>
    public string? NumeroQuittance { get; set; }

    /// <summary>
    /// Mode de paiement (Espèces, Chèque, Virement)
    /// </summary>
    public string ModePaiement { get; set; } = "Espèces";
}