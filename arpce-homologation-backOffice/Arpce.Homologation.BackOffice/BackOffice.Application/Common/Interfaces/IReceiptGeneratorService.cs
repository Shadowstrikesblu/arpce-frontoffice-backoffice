using System;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.Interfaces;

public interface IReceiptGeneratorService
{
    /// <summary>
    /// Génère un reçu de paiement au format PDF pour un dossier donné.
    /// </summary>
    Task<byte[]> GenerateReceiptPdfAsync(Guid dossierId, decimal montant, string modePaiement, string numeroQuittance);
}