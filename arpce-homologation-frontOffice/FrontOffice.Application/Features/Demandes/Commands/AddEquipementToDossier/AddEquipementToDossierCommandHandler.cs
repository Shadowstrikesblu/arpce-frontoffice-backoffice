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

                if (dossier == null) throw new UnauthorizedAccessException("Dossier introuvable.");

                var nouvelleDemande = new Demande
                {
                    Id = Guid.NewGuid(),
                    IdDossier = request.IdDossier,
                    Equipement = request.Equipement,
                    Modele = request.Modele,
                    Marque = request.Marque,
                    Fabricant = request.Fabricant,

                    Type = request.Type,

                    Description = request.Description,
                    QuantiteEquipements = request.QuantiteEquipements,
                    DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    UtilisateurCreation = userId.Value.ToString()
                };

                _context.Demandes.Add(nouvelleDemande);

                await _fileStorageProvider.ImportDocumentDemandeAsync(
                    file: request.TypeURL_FicheTechnique,
                    nom: $"FicheTechnique_{request.Equipement}_{request.Modele}",
                    demandeId: nouvelleDemande.Id
                );

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                await _notificationService.SendToGroupAsync(
                    groupName: "DRSCE",
                    title: "Nouvel Équipement Ajouté",
                    message: $"L'équipement '{nouvelleDemande.Equipement}' (Type: {nouvelleDemande.Type}) a été ajouté au dossier {dossier.Numero}.",
                    type: "Info",
                    targetUrl: $"/dossiers/{dossier.Id}",
                    entityId: dossier.Id.ToString()
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Échec de l'ajout de l'équipement");
                throw;
            }
        });

        return true;
    }
}