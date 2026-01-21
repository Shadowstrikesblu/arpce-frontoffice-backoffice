using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        var demande = await _context.Demandes
            .Include(d => d.Dossier)
            .FirstOrDefaultAsync(d => d.Id == request.DemandeId, cancellationToken);

        if (demande == null) throw new Exception($"Demande introuvable.");

        var categorie = await _context.CategoriesEquipements.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategorieId, cancellationToken);
        if (categorie == null) throw new Exception($"Catégorie introuvable.");

        decimal prixUnitaireCalcule = (categorie.FraisEtude ?? 0) + (categorie.FraisHomologation ?? 0) + (categorie.FraisControle ?? 0);

        demande.IdCategorie = request.CategorieId;
        demande.PrixUnitaire = prixUnitaireCalcule;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Instruction Équipement",
            libelle: $"Assignation de la catégorie '{categorie.Code}' à l'équipement ID '{request.DemandeId}'.",
            eventTypeCode: "MODIFICATION",
            dossierId: demande.IdDossier);

        await _notificationService.SendToGroupAsync(
            profilCode: "DRSCE",
            title: "Mise à Jour Équipement",
            message: $"L'équipement '{demande.Equipement}' du dossier {demande.Dossier.Numero} a été catégorisé.",
            type: "V"
        );

        return true;
    }
}