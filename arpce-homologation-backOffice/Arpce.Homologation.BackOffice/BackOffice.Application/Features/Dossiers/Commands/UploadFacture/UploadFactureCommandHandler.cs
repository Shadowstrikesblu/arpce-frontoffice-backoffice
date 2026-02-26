using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Dossiers.Commands.UploadFacture;

public class UploadFactureCommandHandler : IRequestHandler<UploadFactureCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly IAuditService _auditService;
    private readonly ILogger<UploadFactureCommandHandler> _logger;

    public UploadFactureCommandHandler(IApplicationDbContext context, IFileStorageProvider fileStorageProvider, IAuditService auditService, ILogger<UploadFactureCommandHandler> logger)
    {
        _context = context;
        _fileStorageProvider = fileStorageProvider;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<bool> Handle(UploadFactureCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers.FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);
            if (dossier == null) throw new Exception("Dossier introuvable.");

            if (request.FactureFile == null || request.FactureFile.Length == 0)
                throw new InvalidOperationException("Fichier manquant.");

            await _fileStorageProvider.ImportDocumentDossierAsync(
                file: request.FactureFile,
                nom: $"Facture_{dossier.Numero}",
                type: 2,
                dossierId: dossier.Id,
                libelle: "Facture finale"
            );

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Gestion Dossiers", $"Facture chargée pour {dossier.Numero}", "UPLOAD", dossier.Id);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}