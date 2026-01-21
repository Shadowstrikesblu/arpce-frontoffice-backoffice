using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Demandes.Commands.AddEquipementToDossier;

public class AddEquipementToDossierCommandHandler : IRequestHandler<AddEquipementToDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ILogger<AddEquipementToDossierCommandHandler> _logger;
    private readonly INotificationService _notificationService;

    public AddEquipementToDossierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageProvider fileStorageProvider,
        ILogger<AddEquipementToDossierCommandHandler> logger,
        INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageProvider = fileStorageProvider;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(AddEquipementToDossierCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var dossier = await _context.Dossiers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.IdDossier && d.IdClient == userId.Value, cancellationToken);

                if (dossier == null)
                {
                    throw new UnauthorizedAccessException("Le dossier spécifié est introuvable ou vous n'y avez pas accès.");
                }

                var ficheTechniqueFile = request.TypeURL_FicheTechnique;
                const long maxFileSize = 3 * 1024 * 1024;
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

                if (ficheTechniqueFile == null || ficheTechniqueFile.Length == 0)
                    throw new InvalidOperationException("La fiche technique est obligatoire.");
                if (ficheTechniqueFile.Length > maxFileSize)
                    throw new InvalidOperationException("La taille du fichier ne doit pas dépasser 3 Mo.");
                if (!allowedExtensions.Contains(Path.GetExtension(ficheTechniqueFile.FileName).ToLowerInvariant()))
                    throw new InvalidOperationException("Format de fichier non supporté.");

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

                await _fileStorageProvider.ImportDocumentDemandeAsync(
                    file: ficheTechniqueFile,
                    nom: $"FicheTechnique_{request.Equipement}_{request.Modele}",
                    demandeId: nouvelleDemande.Id
                );

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Équipement {EquipementId} ajouté au dossier {DossierId}", nouvelleDemande.Id, request.IdDossier);

                await _notificationService.SendToGroupAsync(
                    groupName: "DRSCE",
                    title: "Nouvel Équipement Ajouté",
                    message: $"Un nouvel équipement ('{nouvelleDemande.Equipement}') a été ajouté au dossier {dossier.Numero}.",
                    type: "Info",
                    targetUrl: $"/dossiers/{dossier.Id}"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Échec de l'ajout de l'équipement au dossier {DossierId}", request.IdDossier);
                throw;
            }
        });

        return true;
    }
}