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
            // Récupération de l'attestation avec la demande et le dossier
            var attestationToUpdate = await _context.Attestations
                .Include(a => a.Demande)
                    .ThenInclude(d => d.Dossier)
                .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

            if (attestation == null) throw new Exception("Attestation introuvable.");

            var dossier = attestation.Demande.Dossier;

            if (request.CertificatFile == null || request.CertificatFile.Length == 0)
                throw new InvalidOperationException("Fichier PDF obligatoire.");

            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream, cancellationToken);
                fileData = memoryStream.ToArray();
            }

            attestation.Donnees = fileData;
            attestation.Extension = Path.GetExtension(request.CertificatFile.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf";
            attestation.DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds();
            attestation.DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds();

            _context.Attestations.Update(attestation);

            var statutSigneEquip = await _context.Statuts.AsNoTracking().FirstOrDefaultAsync(s => s.Code == "Signe", cancellationToken);
            if (statutSigneEquip != null) attestation.Demande.IdStatut = statutSigneEquip.Id;

            await _context.SaveChangesAsync(cancellationToken);

            // récupère les demandes en mémoire
            var demandesDuDossier = await _context.Demandes
                .AsNoTracking()
                .Include(d => d.Statut)
                .Where(d => d.IdDossier == dossier.Id)
                .ToListAsync(cancellationToken);

            var demandeIds = demandesDuDossier.Select(d => d.Id).ToList();

            // récupère les attestations en mémoire (SANS filtre Donnees en SQL)
            var allAttestations = await _context.Attestations
                .AsNoTracking()
                .Where(a => demandeIds.Contains(a.IdDemande))
                .ToListAsync(cancellationToken);

            // fait le test complexe en C# (L'erreur 422 est impossible ici)
            bool toutEstTraite = demandesDuDossier.All(d =>
                (d.Statut != null && d.Statut.Code == "Refus") ||
                allAttestations.Any(a => a.IdDemande == d.Id && a.Donnees != null && a.Donnees.Length > 0)
            );

            if (demandesDuDossier.Any() && toutEstTraite)
            {
                var statutSigneDossier = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);
                if (statutSigneDossier != null && dossier.IdStatut != statutSigneDossier.Id)
                {
                    dossier.IdStatut = statutSigneDossier.Id;
                    _context.Dossiers.Update(dossier);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Signature", $"Certificat chargé pour {attestation.Demande.Equipement}.", "SIGNATURE", dossier.Id);
            await _notificationService.SendToGroupAsync("DRSCE", "Signature", "Attestation chargée.", "E", $"/dossiers/{dossier.Id}", dossier.Id.ToString());

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erreur upload");
            throw;
        }
    }
}