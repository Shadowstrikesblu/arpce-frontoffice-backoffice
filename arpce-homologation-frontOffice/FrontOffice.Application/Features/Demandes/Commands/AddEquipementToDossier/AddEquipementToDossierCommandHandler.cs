using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Demandes.Commands.AddEquipementToDossier;

/// <summary>
/// Gère la logique de la commande pour ajouter un équipement et sa fiche technique à un dossier existant.
/// Utilise IFileStorageProvider pour appeler la procédure stockée d'importation.
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
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Vérifie que le dossier parent existe et appartient bien au client
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
            throw new InvalidOperationException("La fiche technique est obligatoire.");
        }
        if (ficheTechniqueFile.Length > maxFileSize)
        {
            throw new InvalidOperationException($"La taille de la fiche technique ne doit pas dépasser 3 Mo.");
        }
        if (!allowedExtensions.Contains(Path.GetExtension(ficheTechniqueFile.FileName).ToLowerInvariant()))
        {
            throw new InvalidOperationException("Le format du fichier de la fiche technique n'est pas supporté.");
        }

        // Crée l'entité Demande (équipement) en mémoire (sans la sauvegarder encore)
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
            ContactEmail = request.ContactEmail,
            EstHomologable = true 
        };
        _context.Demandes.Add(nouvelleDemande);

        // Utilise le service de stockage pour appeler la procédure stockée
        try
        {
            // On sauvegarde d'abord la demande pour avoir un ID valide sur lequel la procédure va s'appuyer
            await _context.SaveChangesAsync(cancellationToken);

            // Ensuite, on importe le document qui sera lié à cette demande
            await _fileStorageProvider.ImportDocumentDemandeAsync(
                file: ficheTechniqueFile,
                nom: $"FicheTechnique_{request.Equipement}_{request.Modele}",
                demandeId: nouvelleDemande.Id
            );
        }
        catch (Exception ex)
        {
            // En cas d'échec de l'import, on annule la création de la demande pour garder la cohérence.
            // Cette partie est un "rollback manuel".
            _context.Demandes.Remove(nouvelleDemande);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogError(ex, "Échec de l'importation de la fiche technique. La création de la demande a été annulée.");
            throw new InvalidOperationException($"Échec de la création du document : {ex.Message}");
        }

        return true;
    }
}