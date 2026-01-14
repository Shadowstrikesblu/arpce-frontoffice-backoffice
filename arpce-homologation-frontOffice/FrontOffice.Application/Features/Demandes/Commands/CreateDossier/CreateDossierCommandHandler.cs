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

        // On utilise la stratégie d'exécution fournie par EF Core pour gérer les transactions et les nouvelles tentatives.
        var strategy = _context.Database.CreateExecutionStrategy();

        var response = await strategy.ExecuteAsync(async () =>
        {
            // On déclare une transaction manuelle à l'intérieur de la stratégie.
            // EF Core saura comment la gérer en cas de nouvelle tentative.
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Vérifications initiales
                if (await _context.Dossiers.AnyAsync(d => d.Libelle == request.Libelle, cancellationToken))
                {
                    throw new InvalidOperationException($"Un dossier avec le libellé '{request.Libelle}' existe déjà.");
                }

                var file = request.CourrierFile;
                if (file == null || file.Length == 0)
                {
                    throw new InvalidOperationException("La lettre de demande est un fichier obligatoire.");
                }
                const long maxFileSize = 3 * 1024 * 1024;
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

                // Prépare l'entité Dossier
                var dossier = new Dossier
                {
                    Id = Guid.NewGuid(),
                    IdClient = userId.Value,
                    IdStatut = defaultStatut.Id,
                    DateOuverture = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Numero = $"HOM-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                    Libelle = request.Libelle,
                };
                _context.Dossiers.Add(dossier);

                // Prépare l'entité Document (sans la sauvegarder)
                await _fileStorageProvider.ImportDocumentDossierAsync(
                    file: request.CourrierFile,
                    nom: $"Lettre_Demande_{dossier.Numero}",
                    type: 0,
                    dossierId: dossier.Id
                );

                await _context.SaveChangesAsync(cancellationToken);

                // Valide la transaction si tout s'est bien passé
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Dossier {DossierId} et son document créés avec succès.", dossier.Id);

                return new CreateDossierResponseDto { DossierId = dossier.Id };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Échec de la création du dossier. Transaction annulée.");
                throw; 
            }
        });

        return response;
    }
}