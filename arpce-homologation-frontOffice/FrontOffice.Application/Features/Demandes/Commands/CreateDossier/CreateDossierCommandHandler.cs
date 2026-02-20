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
    private readonly INotificationService _notificationService;

    public CreateDossierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageProvider fileStorageProvider,
        ILogger<CreateDossierCommandHandler> logger,
        INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageProvider = fileStorageProvider;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<CreateDossierResponseDto> Handle(CreateDossierCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var strategy = _context.Database.CreateExecutionStrategy();
        var response = await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (await _context.Dossiers.AsNoTracking().AnyAsync(d => d.Libelle == request.Libelle && d.IdClient == userId.Value, cancellationToken))
                    throw new InvalidOperationException($"Un dossier avec le libellé '{request.Libelle}' existe déjà pour votre compte.");

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
                    throw new InvalidOperationException("Le fichier de la lettre de demande doit être au format PDF.");
                }

                var defaultStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.NouveauDossier.ToString(), cancellationToken);
                if (defaultStatut == null) throw new InvalidOperationException("Configuration système manquante : le statut par défaut 'NouveauDossier' est introuvable.");

                var currentYear = DateTime.UtcNow.ToString("yy"); 
                var prefix = $"Hom-{currentYear}-";

                // On compte le nombre de dossiers créés pour l'année en cours pour incrémenter la séquence
                var countThisYear = await _context.Dossiers
                    .AsNoTracking()
                    .CountAsync(d => d.Numero.StartsWith(prefix), cancellationToken);

                var sequence = (countThisYear + 1).ToString("D4"); 
                var generatedNumero = $"{prefix}{sequence}";

                var dossier = new Dossier
                {
                    Id = Guid.NewGuid(),
                    IdClient = userId.Value,
                    IdStatut = defaultStatut.Id,
                    DateOuverture = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Numero = generatedNumero,
                    Libelle = request.Libelle,
                    UtilisateurCreation = userId.Value.ToString(),
                    DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                _context.Dossiers.Add(dossier);

                // Importation du document avec le nouveau numéro
                await _fileStorageProvider.ImportDocumentDossierAsync(file, $"Lettre_Demande_{dossier.Numero}", 0, dossier.Id);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Dossier {DossierId} créé avec le numéro {Numero}.", dossier.Id, dossier.Numero);

                await _notificationService.SendToGroupAsync(
                    groupName: "DRSCE",
                    title: "Nouveau Dossier",
                    message: $"Le dossier '{dossier.Libelle}' ({dossier.Numero}) vient d'être soumis.",
                    type: "V",
                    targetUrl: $"/dossiers/{dossier.Id}",
                    entityId: dossier.Id.ToString()
                );

                return new CreateDossierResponseDto { DossierId = dossier.Id };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Échec de la création du dossier.");
                throw;
            }
        });
        return response;
    }
}