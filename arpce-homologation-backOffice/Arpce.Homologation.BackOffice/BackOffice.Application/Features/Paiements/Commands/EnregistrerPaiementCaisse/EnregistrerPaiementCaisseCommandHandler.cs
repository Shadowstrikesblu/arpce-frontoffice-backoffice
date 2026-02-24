using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<EnregistrerPaiementCaisseCommandHandler> _logger;

    public EnregistrerPaiementCaisseCommandHandler(
        IApplicationDbContext context,
        IReceiptGeneratorService receiptGenerator,
        IFileStorageProvider fileStorage,
        INotificationService notificationService,
        ILogger<EnregistrerPaiementCaisseCommandHandler> logger)
    {
        _context = context;
        _receiptGenerator = receiptGenerator;
        _fileStorage = fileStorage;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(EnregistrerPaiementCaisseCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers
                .Include(d => d.Client)
                .Include(d => d.Demande)
                .FirstOrDefaultAsync(d => d.Numero == request.NumeroDossier, cancellationToken);

            if (dossier == null) throw new Exception($"Dossier N° {request.NumeroDossier} introuvable.");

            var quittance = request.NumeroQuittance ?? $"Q-{DateTime.UtcNow:yyyyMMddHHmm}";
            byte[] pdfBytes = await _receiptGenerator.GenerateReceiptPdfAsync(
                dossier.Id,
                request.MontantEncaisse,
                request.ModePaiement,
                quittance);

            await _fileStorage.SaveDocumentDossierFromBytesAsync(
                pdfBytes,
                $"Recu_Caisse_{dossier.Numero}.pdf",
                3,
                dossier.Id,
                "Reçu de paiement caisse officiel");

            var devis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == dossier.Id, cancellationToken);
            if (devis != null)
            {
                devis.PaiementOk = 1; 
                devis.PaiementMobileId = quittance; 
            }

            var statutCaisse = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "PaiementCaisse", cancellationToken);
            if (statutCaisse != null)
            {
                dossier.IdStatut = statutCaisse.Id;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _notificationService.SendEventNotificationAsync(NotificationEvent.NouveauPaiement, dossier.Id);

            _logger.LogInformation("Paiement caisse enregistré pour le dossier {Num}", dossier.Numero);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erreur lors du paiement caisse pour {Num}", request.NumeroDossier);
            throw;
        }
    }
}