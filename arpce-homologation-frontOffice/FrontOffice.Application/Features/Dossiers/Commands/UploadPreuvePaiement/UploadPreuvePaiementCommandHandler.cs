using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using System;
using System.IO; 
using System.Threading;
using System.Threading.Tasks;

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
            // Vous pouvez ajouter d'autres validations (taille, extension...) ici

            // Utilise le service de stockage pour importer la preuve
            await _fileStorageProvider.ImportDocumentDossierAsync(
                file: file,
                nom: $"PreuvePaiement_{dossier.Numero}",
                type: 3, 
                dossierId: dossier.Id
            );

            // Optionnel : Changer le statut du dossier à "En attente de validation de paiement"
            // var statutValidationPaiement = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "PaiementEnValidation", cancellationToken);
            // if (statutValidationPaiement != null)
            // {
            //     dossier.IdStatut = statutValidationPaiement.Id;
            //     await _context.SaveChangesAsync(cancellationToken);
            // }

            _logger.LogInformation("Preuve de paiement uploadée pour le dossier {DossierId} par l'utilisateur {UserId}", dossier.Id, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Échec de l'upload de la preuve de paiement pour le dossier {DossierId}", request.DossierId);
            throw;
        }
    }
}