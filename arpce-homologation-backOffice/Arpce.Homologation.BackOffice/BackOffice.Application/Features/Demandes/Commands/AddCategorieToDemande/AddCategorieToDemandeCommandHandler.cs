using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Commands.AddCategorieToDemande;

/// <summary>
/// Gère la logique de la commande pour assigner une catégorie à une demande.
/// **Logique métier ajoutée :** Met à jour automatiquement le PrixUnitaire de l'équipement
/// en se basant sur les frais de la catégorie assignée.
/// </summary>
public class AddCategorieToDemandeCommandHandler : IRequestHandler<AddCategorieToDemandeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AddCategorieToDemandeCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public AddCategorieToDemandeCommandHandler(
        IApplicationDbContext context,
        ILogger<AddCategorieToDemandeCommandHandler> logger,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(AddCategorieToDemandeCommand request, CancellationToken cancellationToken)
    {
        // Récupére la demande (équipement)
        var demande = await _context.Demandes.FindAsync(new object[] { request.DemandeId }, cancellationToken);
        if (demande == null)
        {
            throw new Exception($"La demande (équipement) avec l'ID '{request.DemandeId}' est introuvable.");
        }

        // Récupére la catégorie complète (avec ses frais)
        var categorie = await _context.CategoriesEquipements
            .AsNoTracking() 
            .FirstOrDefaultAsync(c => c.Id == request.CategorieId, cancellationToken);

        if (categorie == null)
        {
            throw new Exception($"La catégorie avec l'ID '{request.CategorieId}' est introuvable.");
        }

        // Calcule le prix total à partir des frais de la catégorie
        // Le prix unitaire est la somme des frais d'étude, d'homologation et de contrôle.
        decimal prixUnitaireCalcule = (categorie.FraisEtude ?? 0) +
                                       (categorie.FraisHomologation ?? 0) +
                                       (categorie.FraisControle ?? 0);

        // Met à jour la demande
        demande.IdCategorie = request.CategorieId;
        demande.PrixUnitaire = prixUnitaireCalcule; 

        _logger.LogInformation(
            "Catégorie {CategorieId} assignée à la demande {DemandeId}. Prix unitaire mis à jour à {Prix}",
            request.CategorieId, request.DemandeId, prixUnitaireCalcule);

        // Sauvegarde les changements
        await _context.SaveChangesAsync(cancellationToken);

        // Journalise l'action
        await _auditService.LogAsync(
            page: "Instruction Équipement",
            libelle: $"Assignation de la catégorie '{categorie.Code}' et mise à jour du prix à {prixUnitaireCalcule} pour l'équipement ID '{request.DemandeId}'.",
            eventTypeCode: "MODIFICATION",
            dossierId: demande.IdDossier);

        return true;
    }
}