using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Dossiers.Commands.ValidateInstruction;

public class ValidateInstructionCommandHandler : IRequestHandler<ValidateInstructionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ValidateInstructionCommandHandler> _logger;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public ValidateInstructionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ValidateInstructionCommandHandler> logger,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(ValidateInstructionCommand request, CancellationToken cancellationToken)
    {
        var agentId = _currentUserService.UserId;
        if (!agentId.HasValue)
        {
            throw new UnauthorizedAccessException("Accès non autorisé. L'authentification de l'agent est requise.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Récupére le dossier avec ses demandes et les catégories associées pour le calcul
            var dossier = await _context.Dossiers
                .Include(d => d.Demandes)
                    .ThenInclude(dem => dem.CategorieEquipement)
                .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

            if (dossier == null)
            {
                throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");
            }

            _logger.LogInformation("Début de la création du devis pour le dossier {DossierId}", dossier.Id);

            // Vérifie s'il existe déjà un devis non payé pour ce dossier
            var existingDevis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == dossier.Id && d.PaiementOk != 1, cancellationToken);
            if (existingDevis != null)
            {
                _logger.LogWarning("Un devis non payé existe déjà pour le dossier {DossierId}. La création d'un nouveau devis est annulée.", dossier.Id);
            }
            else
            {
                decimal totalFraisEtude = 0;
                decimal totalFraisHomologation = 0;
                decimal totalFraisControle = 0;

                foreach (var demande in dossier.Demandes)
                {
                    if (demande.CategorieEquipement != null)
                    {
                        totalFraisEtude += demande.CategorieEquipement.FraisEtude ?? 0;
                        totalFraisHomologation += demande.CategorieEquipement.FraisHomologation ?? 0;
                        totalFraisControle += demande.CategorieEquipement.FraisControle ?? 0;
                    }
                    else
                    {
                        _logger.LogWarning("La demande {DemandeId} n'a pas de catégorie. Ses frais ne seront pas inclus.", demande.Id);
                    }
                }

                var nouveauDevis = new Devis
                {
                    Id = Guid.NewGuid(),
                    IdDossier = dossier.Id,
                    Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    MontantEtude = totalFraisEtude,
                    MontantHomologation = totalFraisHomologation,
                    MontantControle = totalFraisControle,
                    PaiementOk = 0
                };
                _context.Devis.Add(nouveauDevis);
                _logger.LogInformation("Devis créé en mémoire pour le dossier {DossierId}.", dossier.Id);
            }

            // Met à jour le statut du dossier vers "InstructionApprouve"
            var statutCibleCode = StatutDossierEnum.InstructionApprouve.ToString();
            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == statutCibleCode, cancellationToken);
            if (nouveauStatut == null)
            {
                throw new Exception($"Configuration système manquante : le statut '{statutCibleCode}' est introuvable.");
            }
            dossier.IdStatut = nouveauStatut.Id;
            _logger.LogInformation("Le statut du dossier {DossierId} a été changé à '{NouveauStatut}'.", dossier.Id, nouveauStatut.Libelle);

            if (!string.IsNullOrWhiteSpace(request.Remarque))
            {
                var agent = await _context.AdminUtilisateurs.FindAsync(agentId.Value);
                var nomAgent = agent != null ? $"{agent.Prenoms} {agent.Nom}" : "Agent Inconnu";

                var commentaire = new Commentaire
                {
                    Id = Guid.NewGuid(),
                    IdDossier = dossier.Id,
                    DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    CommentaireTexte = request.Remarque,
                    NomInstructeur = nomAgent
                };
                _context.Commentaires.Add(commentaire);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync(
                page: "Validation Dossier",
                libelle: $"Instruction du dossier '{dossier.Numero}' approuvée et devis généré.",
                eventTypeCode: "VALIDATION",
                dossierId: dossier.Id);

            await _notificationService.SendToGroupAsync(
                profilCode: "DOSSIERS",
                title: "Instruction Approuvée",
                message: $"L'instruction pour le dossier {dossier.Numero} a été approuvée. Le devis est prêt.",
                type: "V",
                targetUrl: $"/dossiers/{dossier.Id}",
                entityId: dossier.Id.ToString()
            );

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec de la validation de l'instruction pour le dossier {DossierId}", request.DossierId);
            throw;
        }
    }
}