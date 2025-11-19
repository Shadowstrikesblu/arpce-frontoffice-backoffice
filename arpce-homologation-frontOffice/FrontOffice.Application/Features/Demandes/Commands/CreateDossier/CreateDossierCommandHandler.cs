using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
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

    /// <summary>
    /// Exécute la logique de création d'un dossier.
    /// </summary>
    public async Task<CreateDossierResponseDto> Handle(CreateDossierCommand request, CancellationToken cancellationToken)
    {
        // Étape 1 : Vérifier que l'utilisateur est bien authentifié
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Étape 2 : Valider les données d'entrée
        // Validation de l'unicité du libellé pour éviter les doublons
        if (await _context.Dossiers.AnyAsync(d => d.Libelle == request.Libelle, cancellationToken))
        {
            throw new InvalidOperationException($"Un dossier avec le libellé '{request.Libelle}' existe déjà.");
        }

        // Validation du fichier uploadé (la lettre de demande)
        var file = request.CourrierFile;
        const long maxFileSize = 3 * 1024 * 1024; // Limite de taille fixée à 3 Mo

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

        // Sauvegarder le fichier de manière sécurisée
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}"; 
        string relativeFilePath;
        try
        {
            using (var fileStream = file.OpenReadStream())
            {
                // Appel au service de stockage pour sauvegarder le fichier dans le sous-dossier "courriers"
                relativeFilePath = await _fileStorageProvider.SaveFileAsync(fileStream, uniqueFileName, "courriers");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du stockage de la lettre de demande : {FileName}", file.FileName);
            throw new InvalidOperationException("Une erreur est survenue lors de l'upload du fichier.");
        }

        // Préparation les données pour la création du dossier en base de données
        var defaultStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.NouvelleDemande.ToString(), cancellationToken);
        var defaultModeReglement = await _context.ModesReglements.FirstOrDefaultAsync(mr => mr.Code == ModeReglementEnum.Virement.ToString(), cancellationToken);

        if (defaultStatut == null || defaultModeReglement == null)
        {
            throw new InvalidOperationException("Configuration système manquante : le statut ou le mode de règlement par défaut est introuvable.");
        }

        // Création de l'entité Dossier
        var dossier = new Dossier
        {
            Id = Guid.NewGuid(),
            IdClient = userId.Value,
            IdStatut = defaultStatut.Id,
            IdModeReglement = defaultModeReglement.Id,
            DateOuverture = DateTime.UtcNow,
            Numero = $"HOM-{DateTime.UtcNow:yyyyMMddHHmmssfff}", 
            Libelle = request.Libelle,
        };
        _context.Dossiers.Add(dossier);

        // Création de l'entité DocumentDossier pour lier le fichier au dossier
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

        // Sauvegarde de toutes les modifications dans une seule transaction atomique
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateDossierResponseDto { DossierId = dossier.Id };
    }
}