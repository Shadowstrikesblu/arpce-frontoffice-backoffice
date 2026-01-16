using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums; 
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Dossiers.Commands.UploadPreuvePaiement;

public class UploadPreuvePaiementCommandHandler : IRequestHandler<UploadPreuvePaiementCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger<UploadPreuvePaiementCommandHandler> _logger;

    public UploadPreuvePaiementCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageProvider fileStorageProvider,
        ILogger<UploadPreuvePaiementCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageProvider = fileStorageProvider;
        _logger = logger;
    }

    public async Task<bool> Handle(UploadPreuvePaiementCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Vérifie que le dossier appartient bien à l'utilisateur
            var dossier = await _context.Dossiers.FirstOrDefaultAsync(d => d.Id == request.DossierId && d.IdClient == userId, cancellationToken);
            if (dossier == null)
            {
                throw new Exception("Dossier introuvable ou vous n'avez pas les droits nécessaires.");
            }

            // Valide le fichier
            var file = request.PreuvePaiement;
            if (file == null || file.Length == 0) throw new InvalidOperationException("Le fichier de preuve de paiement est manquant.");

            // Validation de la taille (ex: 5 Mo)
            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize) throw new InvalidOperationException("La taille du fichier ne doit pas dépasser 5 Mo.");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant())) throw new InvalidOperationException("Format de fichier non supporté (PDF, JPG, PNG autorisés).");

            // Utilise le service de stockage pour importer la preuve
            await _fileStorageProvider.ImportDocumentDossierAsync(
                file: file,
                nom: $"PreuvePaiement_{dossier.Numero}",
                type: 3, 
                dossierId: dossier.Id
            );

            var statutCibleCode = StatutDossierEnum.PaiementBanque.ToString();
            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == statutCibleCode, cancellationToken);

            if (nouveauStatut == null)
            {
                _logger.LogError("Le statut '{StatutCode}' est introuvable. Impossible de mettre à jour le statut du dossier.", statutCibleCode);
                throw new Exception($"Configuration système manquante : le statut '{statutCibleCode}' est introuvable.");
            }

            dossier.IdStatut = nouveauStatut.Id;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Preuve de paiement uploadée et statut du dossier {DossierId} mis à jour à '{StatutCode}'.", dossier.Id, statutCibleCode);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec de l'upload de la preuve de paiement pour le dossier {DossierId}", request.DossierId);
            throw;
        }
    }
}