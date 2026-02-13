using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Commands.AddCategorieToDemande;

public class AddCategorieToDemandeCommandHandler : IRequestHandler<AddCategorieToDemandeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AddCategorieToDemandeCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public AddCategorieToDemandeCommandHandler(
        IApplicationDbContext context,
        ILogger<AddCategorieToDemandeCommandHandler> logger,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(AddCategorieToDemandeCommand request, CancellationToken cancellationToken)
    {
        // Recherche de l'équipement (Demande)
        var demande = await _context.Demandes
            .Include(d => d.Dossier)
            .FirstOrDefaultAsync(d => d.Id == request.DemandeId, cancellationToken);

        if (demande == null) throw new Exception($"Demande (Équipement) introuvable.");

        string actionLibelle;

        // CAS : DISSOCIATION (Si CategorieId est null)
        if (!request.CategorieId.HasValue)
        {
            _logger.LogInformation("Dissociation de la catégorie pour l'équipement {DemandeId}", request.DemandeId);

            demande.IdCategorie = null;
            demande.PrixUnitaire = 0; 

            actionLibelle = $"Dissociation de la catégorie pour l'équipement '{demande.Equipement}'.";
        }
        // CAS : ASSOCIATION (Si CategorieId est fourni)
        else
        {
            var categorie = await _context.CategoriesEquipements.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CategorieId.Value, cancellationToken);

            if (categorie == null) throw new Exception($"Catégorie sélectionnée introuvable.");

            // Calcul du prix basé sur les frais de la nouvelle catégorie
            decimal prixUnitaireCalcule = (categorie.FraisEtude ?? 0) +
                                           (categorie.FraisHomologation ?? 0) +
                                           (categorie.FraisControle ?? 0);

            demande.IdCategorie = request.CategorieId.Value;
            demande.PrixUnitaire = prixUnitaireCalcule;

            actionLibelle = $"Assignation de la catégorie '{categorie.Code}' à l'équipement '{demande.Equipement}' (Prix mis à jour : {prixUnitaireCalcule}).";
        }

        // Sauvegarde des changements
        await _context.SaveChangesAsync(cancellationToken);

        // Audit
        await _auditService.LogAsync(
            page: "Instruction Équipement",
            libelle: actionLibelle,
            eventTypeCode: "MODIFICATION",
            dossierId: demande.IdDossier);

        // Notification SignalR (Direction Technique)
        await _notificationService.SendToGroupAsync(
            profilCode: "DRSCE",
            title: "Mise à Jour Équipement",
            message: actionLibelle,
            type: "V",
            targetUrl: $"/dossiers/{demande.IdDossier}",
            entityId: demande.IdDossier.ToString()
        );

        return true;
    }
}