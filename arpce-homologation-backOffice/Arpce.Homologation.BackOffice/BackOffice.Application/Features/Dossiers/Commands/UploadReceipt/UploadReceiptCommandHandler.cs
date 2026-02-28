using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Dossiers.Commands.UploadReceipt;

public class UploadReceiptCommandHandler : IRequestHandler<UploadReceiptCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly IAuditService _auditService;
    private readonly ILogger<UploadReceiptCommandHandler> _logger;

    public UploadReceiptCommandHandler(
        IApplicationDbContext context,
        IFileStorageProvider fileStorageProvider,
        IAuditService auditService,
        ILogger<UploadReceiptCommandHandler> logger)
    {
        _context = context;
        _fileStorageProvider = fileStorageProvider;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<bool> Handle(UploadReceiptCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers.FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);
            if (dossier == null) throw new Exception("Dossier introuvable.");

            if (request.RecuFile == null || request.RecuFile.Length == 0)
                throw new InvalidOperationException("Fichier du reçu manquant.");

            await _fileStorageProvider.ImportDocumentDossierAsync(
                file: request.RecuFile,
                nom: $"Recu_Signe_{dossier.Numero}",
                type: 3,
                dossierId: dossier.Id,
                libelle: "Reçu de paiement signé et scanné"
            );

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Trésorerie", $"Reçu signé chargé pour le dossier {dossier.Numero}", "UPLOAD", dossier.Id);

            _logger.LogInformation("Reçu signé uploadé avec succès pour le dossier {Numero}", dossier.Numero);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erreur lors de l'upload du reçu pour le dossier {DossierId}", request.DossierId);
            throw;
        }
    }
}