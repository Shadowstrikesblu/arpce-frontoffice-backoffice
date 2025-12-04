using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Demandes.Commands.CreateDossier;

/// <summary>
/// Gère la logique de la commande pour créer un nouveau dossier d'homologation.
/// </summary>
public class CreateDossierCommandHandler : IRequestHandler<CreateDossierCommand, CreateDossierResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger<CreateDossierCommandHandler> _logger;

    public CreateDossierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageProvider fileStorageProvider,
        ILogger<CreateDossierCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageProvider = fileStorageProvider;
        _logger = logger;
    }

    public async Task<CreateDossierResponseDto> Handle(CreateDossierCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        if (await _context.Dossiers.AnyAsync(d => d.Libelle == request.Libelle, cancellationToken))
        {
            throw new InvalidOperationException($"Un dossier avec le libellé '{request.Libelle}' existe déjà.");
        }

        var file = request.CourrierFile;
        const long maxFileSize = 3 * 1024 * 1024;

        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException("La lettre de demande est un fichier obligatoire.");
        }
        if (file.Length > maxFileSize)
        {
            throw new InvalidOperationException($"La taille du fichier ne doit pas dépasser 3 Mo.");
        }
        if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pdf")
        {
            throw new InvalidOperationException("Le fichier doit être au format PDF.");
        }

        var defaultStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.NouveauDossier.ToString(), cancellationToken);
        if (defaultStatut == null)
        {
            throw new InvalidOperationException($"Configuration système manquante : le statut par défaut '{StatutDossierEnum.NouveauDossier}' est introuvable.");
        }

        // Créer l'entité Dossier en mémoire
        var dossier = new Dossier
        {
            Id = Guid.NewGuid(),
            IdClient = userId.Value,
            IdStatut = defaultStatut.Id,
            IdModeReglement = null,
            DateOuverture = DateTime.UtcNow,
            Numero = $"HOM-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Libelle = request.Libelle,
        };

        _context.Dossiers.Add(dossier);

        // Ceci crée la ligne dans la table 'dossiers', ce qui rend son ID valide pour la contrainte de clé étrangère.
        await _context.SaveChangesAsync(cancellationToken);

        // Importer le document (maintenant que le dossier parent existe en base)
        try
        {
            await _fileStorageProvider.ImportDocumentDossierAsync(
                file: request.CourrierFile,
                nom: $"Lettre_Demande_{dossier.Numero}",
                type: 0, // Type "0" pour "courrier de demande"
                dossierId: dossier.Id
            );
        }
        catch (Exception ex)
        {
            // Si l'importation du fichier échoue, on doit annuler la création du dossier (Rollback manuel).
            _context.Dossiers.Remove(dossier);
            await _context.SaveChangesAsync(cancellationToken); // Sauvegarde l'annulation

            _logger.LogError(ex, "Échec de l'importation du document via la procédure stockée. La création du dossier a été annulée.");
            throw new InvalidOperationException($"Échec de la création du document : {ex.Message}");
        }

        return new CreateDossierResponseDto { DossierId = dossier.Id };
    }
}