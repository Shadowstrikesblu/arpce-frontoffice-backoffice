using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Paiements.Commands.EnregistrerPaiementCaisse;

public class EnregistrerPaiementCaisseCommandHandler : IRequestHandler<EnregistrerPaiementCaisseCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IReceiptGeneratorService _receiptGenerator;
    private readonly IFileStorageProvider _fileStorage;
    private readonly INotificationService _notificationService;

    public EnregistrerPaiementCaisseCommandHandler(
        IApplicationDbContext context,
        IReceiptGeneratorService receiptGenerator,
        IFileStorageProvider fileStorage,
        INotificationService notificationService)
    {
        _context = context;
        _receiptGenerator = receiptGenerator;
        _fileStorage = fileStorage;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(EnregistrerPaiementCaisseCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Recherche le dossier par son numéro unique (Hom-YY-XXXX)
            var dossier = await _context.Dossiers
                .Include(d => d.Demande)
                .Include(d => d.Client)
                .FirstOrDefaultAsync(d => d.Numero == request.NumeroDossier, cancellationToken);

            if (dossier == null)
                throw new Exception($"Dossier N° {request.NumeroDossier} introuvable.");

            // Générer le Reçu PDF (Contenu binaire)
            var quittance = request.NumeroQuittance ?? $"Q-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100, 999)}";
            byte[] pdfBytes = await _receiptGenerator.GenerateReceiptPdfAsync(
                dossier.Id,
                request.MontantEncaisse,
                request.ModePaiement,
                quittance);

            // Persister le document via le FileStorage 
            await _fileStorage.SaveDocumentDossierFromBytesAsync(
                pdfBytes,
                $"Recu_Caisse_{dossier.Numero}.pdf",
                3, // Type 3 = Preuve de paiement
                dossier.Id);

            // Mise à jour du Devis (Statut Payé)
            var devis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == dossier.Id, cancellationToken);
            if (devis != null)
            {
                devis.PaiementOk = 1;
                devis.PaiementMobileId = quittance;
            }

            // Mise à jour du Statut du Dossier
            var statutCaisse = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "PaiementCaisse", cancellationToken);
            if (statutCaisse != null)
            {
                dossier.IdStatut = statutCaisse.Id;
            }

            // Sauvegarde et Validation
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Notification Centralisée
            await _notificationService.SendEventNotificationAsync(NotificationEvent.NouveauPaiement, dossier.Id);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}