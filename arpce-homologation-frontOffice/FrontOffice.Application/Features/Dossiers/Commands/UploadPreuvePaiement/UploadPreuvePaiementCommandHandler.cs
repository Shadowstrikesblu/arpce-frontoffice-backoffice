using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Dossiers.Commands.UploadPreuvePaiement;

public class UploadPreuvePaiementCommandHandler : IRequestHandler<UploadPreuvePaiementCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger<UploadPreuvePaiementCommandHandler> _logger;
    private readonly INotificationService _notificationService; 

    public UploadPreuvePaiementCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageProvider fileStorageProvider,
        ILogger<UploadPreuvePaiementCommandHandler> logger,
        INotificationService notificationService) 
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageProvider = fileStorageProvider;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(UploadPreuvePaiementCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers.FirstOrDefaultAsync(d => d.Id == request.DossierId && d.IdClient == userId, cancellationToken);
            if (dossier == null) throw new Exception("Dossier introuvable ou non autorisé.");

            var file = request.PreuvePaiement;
            if (file == null || file.Length == 0) throw new InvalidOperationException("Fichier de preuve de paiement manquant.");
            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize) throw new InvalidOperationException("La taille du fichier ne doit pas dépasser 5 Mo.");
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant())) throw new InvalidOperationException("Format de fichier non supporté.");

            await _fileStorageProvider.ImportDocumentDossierAsync(file, $"PreuvePaiement_{dossier.Numero}", 3, dossier.Id);

            var statutCibleCode = StatutDossierEnum.PaiementBanque.ToString();
            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == statutCibleCode, cancellationToken);
            if (nouveauStatut == null) throw new Exception($"Statut '{statutCibleCode}' introuvable.");

            dossier.IdStatut = nouveauStatut.Id;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Preuve de paiement uploadée pour dossier {DossierId}.", dossier.Id);

            await _notificationService.SendToGroupAsync(
                groupName: "DAFC", 
                title: "Preuve de Paiement Soumise",
                message: $"Le client a soumis une preuve de paiement pour le dossier {dossier.Numero}.",
                type: "E", 
                targetUrl: $"/dossiers/{dossier.Id}"
            );

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