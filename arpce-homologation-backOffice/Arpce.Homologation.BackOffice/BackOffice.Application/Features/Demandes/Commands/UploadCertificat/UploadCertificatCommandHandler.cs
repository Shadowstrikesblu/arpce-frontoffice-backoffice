using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Commands.UploadCertificat;

public class UploadCertificatCommandHandler : IRequestHandler<UploadCertificatCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UploadCertificatCommandHandler> _logger;

    public UploadCertificatCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService,
        ILogger<UploadCertificatCommandHandler> logger)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(UploadCertificatCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var attestation = await _context.Attestations
                .Include(a => a.Demande)
                    .ThenInclude(d => d.Dossier)
                .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

            if (attestation == null)
            {
                throw new Exception($"Attestation '{request.AttestationId}' introuvable.");
            }

            if (string.IsNullOrWhiteSpace(attestation.VisaReference))
            {
                // Détermine l'année en cours 
                var anneeStr = DateTime.Now.ToString("yy");
                var anneeFull = DateTime.Now.Year;

                // Calcule la séquence 
                // On compte combien d'attestations ont déjà une référence pour cette année
                var sequence = await _context.Attestations
                    .CountAsync(a => a.VisaReference != null &&
                                     a.VisaReference.EndsWith("/" + anneeStr), cancellationToken) + 1;

                // Construire la référence officielle 
                attestation.VisaReference = $"N°{sequence}/ARPCE-DG/DAJI/DRSCE/{anneeStr}";

                _logger.LogInformation("Génération automatique du nouveau visa : {Visa}", attestation.VisaReference);
            }
            else
            {
                _logger.LogInformation("Réutilisation du visa immuable existant : {Visa}", attestation.VisaReference);
            }

            var visaFinal = attestation.VisaReference;
            var dossier = attestation.Demande.Dossier;

            // Lecture du fichier
            if (request.CertificatFile == null || request.CertificatFile.Length == 0)
                throw new InvalidOperationException("Fichier PDF obligatoire.");

            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await request.CertificatFile.CopyToAsync(memoryStream, cancellationToken);
                fileData = memoryStream.ToArray();
            }

            // Mise à jour de l'attestation
            attestation.Donnees = fileData;
            attestation.Extension = Path.GetExtension(request.CertificatFile.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf";
            attestation.DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds();
            attestation.DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds();

            _context.Attestations.Update(attestation);
            await _context.SaveChangesAsync(cancellationToken);

            // Vérification complétude dossier
            var demandeIds = await _context.Demandes
                .Where(d => d.IdDossier == dossier.Id)
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);

            var completes = await _context.Attestations
                .CountAsync(a => demandeIds.Contains(a.IdDemande) && a.Donnees != null && a.Donnees.Length > 0, cancellationToken);

            if (demandeIds.Count > 0 && completes == demandeIds.Count)
            {
                var statut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);
                if (statut != null) dossier.IdStatut = statut.Id;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Audit et Notifications
            await _auditService.LogAsync("Signature", $"Visa généré : {visaFinal}", "SIGNATURE", dossier.Id);

            var msg = $"Attestation signée (Visa: {visaFinal}) pour {attestation.Demande.Equipement}.";
            await _notificationService.SendToGroupAsync("DRSCE", "Signature", msg, "E", $"/dossiers/{dossier.Id}", dossier.Id.ToString());
            await _notificationService.SendToGroupAsync("DAJI", "Signature", msg, "V", $"/dossiers/{dossier.Id}", dossier.Id.ToString());

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erreur signature automatique");
            throw;
        }
    }
}