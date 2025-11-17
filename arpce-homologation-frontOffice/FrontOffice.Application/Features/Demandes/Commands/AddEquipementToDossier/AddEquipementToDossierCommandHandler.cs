using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Demandes.Commands.AddEquipementToDossier;

/// <summary>
/// Gère la logique de la commande pour ajouter un équipement à un dossier existant.
/// </summary>
public class AddEquipementToDossierCommandHandler : IRequestHandler<AddEquipementToDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger<AddEquipementToDossierCommandHandler> _logger;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public AddEquipementToDossierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageProvider fileStorageProvider,
        ILogger<AddEquipementToDossierCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageProvider = fileStorageProvider;
        _logger = logger;
    }

    /// <summary>
    /// Exécute la logique d'ajout d'un équipement à un dossier.
    /// </summary>
    public async Task<bool> Handle(AddEquipementToDossierCommand request, CancellationToken cancellationToken)
    {
        // Vérification de l'authentification et les droits de l'utilisateur
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Vérifie que le dossier existe et qu'il appartient bien à l'utilisateur connecté
        var dossier = await _context.Dossiers
            .FirstOrDefaultAsync(d => d.Id == request.IdDossier && d.IdClient == userId.Value, cancellationToken);
        if (dossier == null)
        {
            throw new UnauthorizedAccessException("Le dossier spécifié est introuvable ou vous n'avez pas les droits nécessaires.");
        }

        // Valide le fichier de la fiche technique
        var ficheTechniqueFile = request.TypeURL_FicheTechnique;
        const long maxFileSize = 3 * 1024 * 1024; // 3 Mo
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

        if (ficheTechniqueFile == null || ficheTechniqueFile.Length == 0)
        {
            throw new InvalidOperationException("La fiche technique est un fichier obligatoire.");
        }
        if (ficheTechniqueFile.Length > maxFileSize)
        {
            throw new InvalidOperationException($"La taille de la fiche technique ne doit pas dépasser {maxFileSize / 1024 / 1024} Mo.");
        }

        var fileExtension = Path.GetExtension(ficheTechniqueFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException($"Format de fichier non supporté. Formats autorisés : {string.Join(", ", allowedExtensions)}");
        }

        // On génère un nom de fichier unique basé sur un Guid et l'extension d'origine.
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        string relativeFilePath;
        try
        {
            using (var fileStream = ficheTechniqueFile.OpenReadStream())
            {
                relativeFilePath = await _fileStorageProvider.SaveFileAsync(fileStream, uniqueFileName, "fiches-techniques");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du stockage de la fiche technique : {FileName}", ficheTechniqueFile.FileName);
            throw new InvalidOperationException("Une erreur est survenue lors de l'upload de la fiche technique.");
        }

        // Création de l'entité 'Demande', qui représente l'équipement à homologuer
        var nouvelleDemande = new Demande
        {
            Id = Guid.NewGuid(),
            IdDossier = dossier.Id,
            Equipement = request.Equipement,
            Modele = request.Modele,
            Marque = request.Marque,
            Fabricant = request.Fabricant,
            Description = request.Description,
            QuantiteEquipements = request.QuantiteEquipements,
            ContactNom = request.ContactNom,
            ContactEmail = request.ContactEmail
        };
        _context.Demandes.Add(nouvelleDemande);
        // Sauvegarde intermédiaire pour s'assurer que `nouvelleDemande` a bien un ID avant de l'utiliser.
        await _context.SaveChangesAsync(cancellationToken);

        // Création de l'entité 'DocumentDemande' pour lier la fiche technique à la demande d'équipement
        var documentDemande = new DocumentDemande
        {
            Id = Guid.NewGuid(),
            IdDemande = nouvelleDemande.Id,
            Nom = $"FicheTechnique_{request.Equipement}_{request.Modele}",
            Extension = fileExtension.TrimStart('.'),
            Donnees = null
        };

        documentDemande.FilePath = relativeFilePath;

        _context.DocumentsDemandes.Add(documentDemande);

        // Sauvegarde finale pour lier le document à la demande
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}