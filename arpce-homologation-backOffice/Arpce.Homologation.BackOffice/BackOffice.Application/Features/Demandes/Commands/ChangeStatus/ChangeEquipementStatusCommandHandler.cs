using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        // Utilisation d'une transaction pour garantir l'intégrité (Demande + Commentaire + Audit)
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Récupération de l'équipement (Demande) avec son dossier parent
            var demande = await _context.Demandes
                .Include(d => d.Dossier)
                .FirstOrDefaultAsync(d => d.Id == request.EquipementId, cancellationToken);

            if (demande == null)
            {
                throw new Exception($"L'équipement avec l'ID '{request.EquipementId}' est introuvable.");
            }

            // Récupération du nouveau statut en base par son code (Refus, Signe, Echantillon)
            var nouveauStatut = await _context.Statuts
                .FirstOrDefaultAsync(s => s.Code == request.CodeStatut, cancellationToken);

            if (nouveauStatut == null)
            {
                throw new Exception($"Le statut équipement avec le code '{request.CodeStatut}' est introuvable dans la configuration système.");
            }

            // Mise à jour du statut de l'équipement
            demande.IdStatut = nouveauStatut.Id;

            // Ajout d'un commentaire au dossier parent pour l'historique de l'instruction
            var agentId = _currentUserService.UserId;
            string nomAgent = "Système";

            if (agentId.HasValue)
            {
                var agent = await _context.AdminUtilisateurs.FindAsync(new object[] { agentId.Value }, cancellationToken);
                if (agent != null)
                {
                    nomAgent = $"{agent.Prenoms} {agent.Nom}";
                }
            }

            string texteCommentaire = $"[ACTION ÉQUIPEMENT] L'équipement '{demande.Equipement}' (Modèle: {demande.Modele}) est passé au statut : {nouveauStatut.Libelle}.";
            if (!string.IsNullOrWhiteSpace(request.Commentaire))
            {
                texteCommentaire += $" Note de l'instructeur : {request.Commentaire}";
            }

            var nouveauCommentaire = new Commentaire
            {
                Id = Guid.NewGuid(),
                IdDossier = demande.IdDossier,
                DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CommentaireTexte = texteCommentaire,
                NomInstructeur = nomAgent
            };

            _context.Commentaires.Add(nouveauCommentaire);

            // Sauvegarde des modifications en base
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Statut de l'équipement {EquipementId} mis à jour vers {Statut}",
                demande.Id, nouveauStatut.Code);

            // Journalisation dans l'Audit
            await _auditService.LogAsync(
                page: "Instruction Équipement",
                libelle: $"Changement de statut pour l'équipement '{demande.Equipement}' vers '{nouveauStatut.Libelle}'.",
                eventTypeCode: "MODIFICATION",
                dossierId: demande.IdDossier);

            // Notification SignalR en temps réel (Direction Technique DRSCE)
            await _notificationService.SendToGroupAsync(
                profilCode: "DRSCE",
                title: "Statut Équipement Mis à jour",
                message: $"L'équipement '{demande.Equipement}' du dossier {demande.Dossier.Numero} est désormais : {nouveauStatut.Libelle}",
                type: "V", 
                targetUrl: $"/dossiers/{demande.IdDossier}",
                entityId: demande.IdDossier.ToString()
            );

            return true;
        }
        catch (Exception ex)
        {
            // En cas d'erreur, on annule toutes les modifications (Rollback)
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec lors du changement de statut de l'équipement {EquipementId}", request.EquipementId);
            throw;
        }
    }
}