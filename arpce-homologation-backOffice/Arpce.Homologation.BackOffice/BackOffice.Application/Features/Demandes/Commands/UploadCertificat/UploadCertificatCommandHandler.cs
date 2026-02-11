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
            // Récupération de l'attestation avec la demande et le dossier
            var attestationToUpdate = await _context.Attestations
                .Include(a => a.Demande)
                    .ThenInclude(d => d.Dossier)
                .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

            if (attestationToUpdate == null)
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

            var dossier = attestationToUpdate.Demande.Dossier;

            // Validation du fichier
            var file = request.CertificatFile;
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Fichier certificat manquant pour l'attestation {AttestationId}.", request.AttestationId);
                return false;
            }

            // Lecture du fichier
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream, cancellationToken);
                fileData = memoryStream.ToArray();
            }

            // Mise à jour de l'attestation
            attestationToUpdate.Donnees = fileData;
            attestationToUpdate.Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf";
            attestationToUpdate.DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds();
            attestationToUpdate.DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds();

            _context.Attestations.Update(attestationToUpdate);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Attestation {AttestationId} mise à jour avec succès.",
                request.AttestationId
            );

            // Récupération de TOUTES les attestations liées au dossier
            var attestationsDuDossier = await _context.Attestations
                .Where(a => a.Demande.IdDossier == dossier.Id)
                .ToListAsync(cancellationToken);

            var totalAttestations = attestationsDuDossier.Count;
            var completedAttestations = attestationsDuDossier
                .Count(a => a.Donnees != null && a.Donnees.Length > 0);

            // Changement de statut UNIQUEMENT si toutes les attestations sont uploadées
            if (completedAttestations == totalAttestations && totalAttestations > 0)
            {
                _logger.LogInformation(
                    "Toutes les {Total} attestations du dossier {DossierId} sont uploadées. Mise à jour du statut.",
                    totalAttestations,
                    dossier.Id
                );

                var statutSigne = await _context.Statuts
                    .FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);

                if (statutSigne != null && dossier.IdStatut != statutSigne.Id)
                {
                    dossier.IdStatut = statutSigne.Id;
                    _context.Dossiers.Update(dossier);
                }
            }
            else
            {
                _logger.LogInformation(
                    "{Completed}/{Total} attestations complétées pour le dossier {DossierId}. Statut inchangé.",
                    completedAttestations,
                    totalAttestations,
                    dossier.Id
                );
            }

            // Sauvegarde finale
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Audit
            await _auditService.LogAsync(
                "Gestion Attestations",
                $"Certificat uploadé pour l'équipement '{attestationToUpdate.Demande.Equipement}'.",
                "UPLOAD",
                dossier.Id
            );

            // Notifications
            var message =
                $"Le certificat pour l'équipement '{attestationToUpdate.Demande.Equipement}' du dossier {dossier.Numero} est disponible.";

            await _notificationService.SendToGroupAsync(
                "DRSCE",
                "Certificat Signé",
                message,
                "E",
                $"/dossiers/{dossier.Id}",
                dossier.Id.ToString()
            );

            await _notificationService.SendToGroupAsync(
                "DAJI",
                "Certificat Signé",
                message,
                "V",
                $"/dossiers/{dossier.Id}",
                dossier.Id.ToString()
            );

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(
                ex,
                "Erreur lors de l'upload du certificat pour l'attestation {AttestationId}.",
                request.AttestationId
            );
            return false;
        }
    }
}