using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Commands.RejectEquipement;

public class RejectEquipementCommandHandler : IRequestHandler<RejectEquipementCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public RejectEquipementCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(RejectEquipementCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Récupération de l'équipement et du dossier
            var demande = await _context.Demandes
                .Include(d => d.Dossier)
                .FirstOrDefaultAsync(d => d.Id == request.EquipementId, cancellationToken);

            if (demande == null) throw new Exception("Équipement introuvable.");

            // Gestion du Motif de Rejet
            var motif = await _context.MotifsRejets.FirstOrDefaultAsync(m => m.Code == request.MotifRejet.Code, cancellationToken);
            if (motif == null)
            {
                motif = new MotifRejet
                {
                    Id = Guid.NewGuid(),
                    Code = request.MotifRejet.Code,
                    Libelle = request.MotifRejet.Libelle,
                    Remarques = request.MotifRejet.Remarques,
                    DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                };
                _context.MotifsRejets.Add(motif);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Mise à jour de l'équipement 
            demande.IdMotifRejet = motif.Id;

            var statutRefus = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "Refus", cancellationToken);
            if (statutRefus != null)
            {
                demande.IdStatut = statutRefus.Id;
            }

            // Traçabilité 
            var agentId = _currentUserService.UserId;
            string nomAgent = "Système/Agent";
            if (agentId.HasValue)
            {
                var agent = await _context.AdminUtilisateurs.FindAsync(new object[] { agentId.Value }, cancellationToken);
                if (agent != null) nomAgent = $"{agent.Prenoms} {agent.Nom}";
            }

            var commentaire = new Commentaire
            {
                Id = Guid.NewGuid(),
                IdDossier = demande.IdDossier,
                DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CommentaireTexte = $"ÉQUIPEMENT REFUSÉ : '{demande.Equipement}'. Motif : {motif.Libelle}. Remarques : {motif.Remarques}",
                NomInstructeur = nomAgent
            };
            _context.Commentaires.Add(commentaire);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Audit et Notification
            await _auditService.LogAsync("Instruction Équipement", $"L'équipement '{demande.Equipement}' a été rejeté.", "REJET_PARTIEL", demande.IdDossier);

            await _notificationService.SendToGroupAsync(
                 profilCode: "DRSCE",
                 title: "Refus Équipement",
                 message: $"L'équipement '{demande.Equipement}' du dossier {demande.Dossier.Numero} a été refusé.",
                 type: "E",
                 targetUrl: $"/dossiers/{demande.IdDossier}",
                 entityId: demande.IdDossier.ToString()
             );

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}