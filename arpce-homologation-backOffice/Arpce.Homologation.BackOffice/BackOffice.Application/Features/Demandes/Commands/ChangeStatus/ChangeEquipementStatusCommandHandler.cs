using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Demandes.Commands.ChangeStatus;

public class ChangeEquipementStatusCommandHandler : IRequestHandler<ChangeEquipementStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ChangeEquipementStatusCommandHandler> _logger;

    public ChangeEquipementStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        INotificationService notificationService,
        ILogger<ChangeEquipementStatusCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangeEquipementStatusCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Récupération de l'équipement
            var demande = await _context.Demandes
                .Include(d => d.Dossier)
                .FirstOrDefaultAsync(d => d.Id == request.EquipementId, cancellationToken);

            if (demande == null) throw new Exception("Équipement introuvable.");

            // Récupération du nouveau statut 
            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == request.CodeStatut, cancellationToken);
            if (nouveauStatut == null) throw new Exception($"Statut équipement '{request.CodeStatut}' introuvable.");

            // Mise à jour du statut de l'équipement
            demande.IdStatut = nouveauStatut.Id;

            // Ajout d'un commentaire au dossier parent pour l'historique
            if (!string.IsNullOrWhiteSpace(request.Commentaire))
            {
                var agentId = _currentUserService.UserId;
                string nomAgent = "Système";
                if (agentId.HasValue)
                {
                    var agent = await _context.AdminUtilisateurs.FindAsync(new object[] { agentId.Value }, cancellationToken);
                    if (agent != null) nomAgent = $"{agent.Prenoms} {agent.Nom}";
                }

                _context.Commentaires.Add(new Commentaire
                {
                    Id = Guid.NewGuid(),
                    IdDossier = demande.IdDossier,
                    DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    CommentaireTexte = $"[ÉQUIPEMENT: {demande.Equipement}] Changement statut vers {nouveauStatut.Libelle}. Note: {request.Commentaire}",
                    NomInstructeur = nomAgent
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Audit
            await _auditService.LogAsync(
                "Instruction Équipement",
                $"Statut de l'équipement '{demande.Equipement}' changé vers '{nouveauStatut.Libelle}'.",
                "MODIFICATION",
                demande.IdDossier
            );

            // Notification SignalR (Direction Technique)
            await _notificationService.SendToGroupAsync(
                profilCode: "DRSCE",
                title: "Statut Équipement Mis à jour",
                message: $"L'équipement '{demande.Equipement}' est désormais au statut : {nouveauStatut.Libelle}",
                type: "V",
                targetUrl: $"/dossiers/{demande.IdDossier}",
                entityId: demande.IdDossier.ToString()
            );

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec changement statut équipement {Id}", request.EquipementId);
            throw;
        }
    }
}