using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Demandes.Commands.CreateDossier;

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

        // Validation unicité libellé
        if (await _context.Dossiers.AnyAsync(d => d.Libelle == request.Libelle, cancellationToken))
        {
            throw new InvalidOperationException($"Un dossier avec le libellé '{request.Libelle}' existe déjà.");
        }

        // Validation fichier
        var file = request.CourrierFile;
        const long maxFileSize = 3 * 1024 * 1024;

        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException("La lettre de demande est un fichier obligatoire.");
        }
        if (file.Length > maxFileSize)
        {
            throw new InvalidOperationException($"La taille du fichier ne doit pas dépasser {maxFileSize / 1024 / 1024} Mo.");
        }
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (fileExtension != ".pdf")
        {
            throw new InvalidOperationException("Le fichier de la lettre de demande doit être au format PDF.");
        }

        // Stockage fichier
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        string relativeFilePath;
        try
        {
            using (var fileStream = file.OpenReadStream())
            {
                relativeFilePath = await _fileStorageProvider.SaveFileAsync(fileStream, uniqueFileName, "courriers");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du stockage de la lettre de demande : {FileName}", file.FileName);
            throw new InvalidOperationException("Une erreur est survenue lors de l'upload du fichier.");
        }

        // --- MODIFICATION STATUS : Utilisation de "NouveauDossier" ---
        // On cherche le statut initial défini dans la nouvelle liste
        var defaultStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.NouveauDossier.ToString(), cancellationToken);

        if (defaultStatut == null)
        {
            throw new InvalidOperationException($"Configuration système manquante : le statut par défaut '{StatutDossierEnum.NouveauDossier}' est introuvable.");
        }

        // Création Dossier
        var dossier = new Dossier
        {
            Id = Guid.NewGuid(),
            IdClient = userId.Value,
            IdStatut = defaultStatut.Id,
            IdModeReglement = null, // Nullable
            DateOuverture = DateTime.UtcNow,
            Numero = $"HOM-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Libelle = request.Libelle,
        };
        _context.Dossiers.Add(dossier);

        // Création DocumentDossier
        var documentDossier = new DocumentDossier
        {
            Id = Guid.NewGuid(),
            IdDossier = dossier.Id,
            Nom = $"Lettre_Demande_{dossier.Numero}",
            Type = 0,
            Extension = fileExtension.TrimStart('.'),
            FilePath = relativeFilePath,
            Donnees = null
        };
        _context.DocumentsDossiers.Add(documentDossier);

        await _context.SaveChangesAsync(cancellationToken);

        return new CreateDossierResponseDto { DossierId = dossier.Id };
    }
}