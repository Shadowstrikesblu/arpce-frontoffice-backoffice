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

public class EnregistrerPaiementCaisseCommandHandler : IRequestHandler<EnregistrerPaiementCaisseCommand, PaiementCaisseResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IReceiptGeneratorService _receiptGenerator;
    private readonly INotificationService _notificationService;
    private readonly ILogger<EnregistrerPaiementCaisseCommandHandler> _logger;

    public EnregistrerPaiementCaisseCommandHandler(
        IApplicationDbContext context,
        IReceiptGeneratorService receiptGenerator,
        INotificationService notificationService,
        ILogger<EnregistrerPaiementCaisseCommandHandler> logger)
    {
        _context = context;
        _receiptGenerator = receiptGenerator;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<PaiementCaisseResult> Handle(EnregistrerPaiementCaisseCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers
                .Include(d => d.Client)
                .Include(d => d.Demande)
                .FirstOrDefaultAsync(d => d.Numero == request.NumeroDossier, cancellationToken);

            if (dossier == null)
                throw new Exception($"Dossier N° {request.NumeroDossier} introuvable.");

            var quittance = request.NumeroQuittance ?? $"Q-{DateTime.Now.Ticks.ToString().Substring(10)}";
            byte[] pdfBytes = await _receiptGenerator.GenerateReceiptPdfAsync(
                dossier.Id,
                request.MontantEncaisse,
                request.ModePaiement,
                quittance);

            var documentId = Guid.NewGuid();
            var docRecu = new DocumentDossier
            {
                Id = documentId,
                IdDossier = dossier.Id,
                Nom = $"Recu_Caisse_{dossier.Numero}.pdf",
                Libelle = "Reçu de paiement caisse officiel",
                Type = 3, 
                Extension = "pdf",
                Donnees = pdfBytes,
                DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                UtilisateurCreation = "SYSTEM"
            };

            _context.DocumentsDossiers.Add(docRecu);

            var devis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == dossier.Id, cancellationToken);
            if (devis != null)
            {
                devis.PaiementOk = 1;
                devis.MontantTotal = request.MontantEncaisse;
                devis.PaiementMobileId = quittance;
            }

            var statutCaisse = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "PaiementCaisse", cancellationToken);
            if (statutCaisse != null) dossier.IdStatut = statutCaisse.Id;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _notificationService.SendEventNotificationAsync(NotificationEvent.NouveauPaiement, dossier.Id);

            string downloadPath = $"/api/demandes/dossier/{documentId}/download?type=recu";

            _logger.LogInformation("Paiement caisse réussi pour {Num}, reçu ID: {Id}", dossier.Numero, documentId);

            return new PaiementCaisseResult(documentId, downloadPath);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erreur lors du paiement caisse");
            throw;
        }
    }
}