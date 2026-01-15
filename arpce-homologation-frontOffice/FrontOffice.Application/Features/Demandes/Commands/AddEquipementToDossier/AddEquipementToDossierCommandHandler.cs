using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Demandes.Commands.AddEquipementToDossier;

public class AddEquipementToDossierCommandHandler : IRequestHandler<AddEquipementToDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger<AddEquipementToDossierCommandHandler> _logger;

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

    public async Task<bool> Handle(AddEquipementToDossierCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        // On utilise une transaction pour garantir l'intégrité de l'opération
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Vérifie que le dossier parent existe et appartient bien au client
                var dossierExists = await _context.Dossiers
                    .AnyAsync(d => d.Id == request.IdDossier && d.IdClient == userId.Value, cancellationToken);

                if (!dossierExists)
                {
                    throw new UnauthorizedAccessException("Le dossier spécifié est introuvable ou vous n'y avez pas accès.");
                }

                // Valide le fichier de la fiche technique
                var ficheTechniqueFile = request.TypeURL_FicheTechnique;
                const long maxFileSize = 3 * 1024 * 1024; // 3 Mo
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

                if (ficheTechniqueFile == null || ficheTechniqueFile.Length == 0) throw new InvalidOperationException("La fiche technique est obligatoire.");
                if (ficheTechniqueFile.Length > maxFileSize) throw new InvalidOperationException("La taille du fichier ne doit pas dépasser 3 Mo.");
                if (!allowedExtensions.Contains(Path.GetExtension(ficheTechniqueFile.FileName).ToLowerInvariant())) throw new InvalidOperationException("Format de fichier non supporté.");

                // Crée l'entité Demande (équipement)
                var nouvelleDemande = new Demande
                {
                    Id = Guid.NewGuid(),
                    IdDossier = request.IdDossier,
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

                // Prépare l'entité DocumentDemande (sans la sauvegarder)
                await _fileStorageProvider.ImportDocumentDemandeAsync(
                    file: ficheTechniqueFile,
                    nom: $"FicheTechnique_{request.Equipement}_{request.Modele}",
                    demandeId: nouvelleDemande.Id
                );

                // Sauvegarde TOUT (Demande + Document) en une seule fois
                await _context.SaveChangesAsync(cancellationToken);

                // Valide la transaction
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Équipement {EquipementId} et sa fiche technique ajoutés au dossier {DossierId}", nouvelleDemande.Id, request.IdDossier);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Échec de l'ajout de l'équipement au dossier {DossierId}", request.IdDossier);
                throw; // Relance pour que le middleware gère l'erreur
            }
        });

        return true;
    }
}