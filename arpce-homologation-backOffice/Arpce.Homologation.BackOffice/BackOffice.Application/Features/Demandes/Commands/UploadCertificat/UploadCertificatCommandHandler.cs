using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            var demande = await _context.Demandes
                .Include(d => d.Dossier)
                .FirstOrDefaultAsync(d => d.Id == request.IdDemande, cancellationToken);

            if (demande == null) throw new Exception("Demande introuvable.");

            var file = request.CertificatFile;
            if (file == null || file.Length == 0) throw new InvalidOperationException("Fichier du certificat manquant.");

            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream, cancellationToken);
                fileData = memoryStream.ToArray();
            }

            var attestation = await _context.Attestations.FirstOrDefaultAsync(a => a.IdDemande == demande.Id, cancellationToken);
            if (attestation != null)
            {
                attestation.DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds();
                attestation.DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds();
                attestation.Donnees = fileData;
                attestation.Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf";
                _context.Attestations.Update(attestation);
            }
            else
            {
                // Création d'une nouvelle attestation
                attestation = new Attestation
                {
                    Id = Guid.NewGuid(),
                    IdDemande = demande.Id,
                    DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds(),
                    DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds(),
                    Donnees = fileData,
                    Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf"
                };
                _context.Attestations.Add(attestation);
                _logger.LogInformation("Nouvelle attestation créée pour la demande {DemandeId}", demande.Id);
            }

            var statutSigne = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);
            if (statutSigne != null) demande.Dossier.IdStatut = statutSigne.Id;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Gestion des Attestations", $"Certificat uploadé pour '{demande.Equipement}'.", "UPLOAD", demande.IdDossier);

            // Notification aux groupes DRSCE et DAJI
            var message = $"L'attestation pour le dossier {demande.Dossier.Numero} est signée et disponible.";
            await _notificationService.SendToGroupAsync("DRSCE", "Attestation Signée", message, "E", $"/dossiers/{demande.IdDossier}", demande.IdDossier.ToString());
            await _notificationService.SendToGroupAsync("DAJI", "Attestation Signée", message, "V", $"/dossiers/{demande.IdDossier}", demande.IdDossier.ToString());

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec de l'upload du certificat pour la demande {DemandeId}", request.IdDemande);
            throw;
        }
    }
}