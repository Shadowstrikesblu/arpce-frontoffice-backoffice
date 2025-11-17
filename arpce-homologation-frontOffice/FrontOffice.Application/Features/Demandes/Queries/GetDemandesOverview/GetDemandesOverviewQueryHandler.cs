using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Demandes.Queries.GetDemandesOverview;

/// <summary>
/// Gère la logique de la requête pour obtenir l'aperçu des demandes du client connecté.
/// </summary>
public class GetDemandesOverviewQueryHandler : IRequestHandler<GetDemandesOverviewQuery, DemandesOverviewDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    /// <param name="context">Le contexte de la base de données.</param>
    /// <param name="currentUserService">Le service pour obtenir l'utilisateur connecté.</param>
    public GetDemandesOverviewQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la logique pour obtenir l'aperçu des demandes.
    /// </summary>
    /// <param name="request">La requête pour l'aperçu des demandes.</param>
    /// <param name="cancellationToken">Le token d'annulation.</param>
    /// <returns>Un DTO contenant l'aperçu des demandes.</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si l'utilisateur n'est pas authentifié.</exception>
    public async Task<DemandesOverviewDto> Handle(GetDemandesOverviewQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Récupére tous les dossiers de l'utilisateur connecté
        var dossiers = await _context.Dossiers
            .Where(d => d.IdClient == userId.Value)
            .Include(d => d.Demandes) 
            .Include(d => d.Statut) 
            .Include(d => d.Devis) 
            .ToListAsync(cancellationToken);

        // Définir les codes de statut attendus
        var statutValideCode = "VALID"; 
        var statutRejeteCode = "REJET"; 
        var statutEnCoursCode = "ENCRS"; 
        var statutAttentePaiementCode = "ATTPAI";

        // Calculer les totaux
        var total = dossiers.Count;
        var success = dossiers.Count(d => d.Statut?.Code == statutValideCode);
        var failed = dossiers.Count(d => d.Statut?.Code == statutRejeteCode);
        var inProgress = dossiers.Count(d => d.Statut?.Code == statutEnCoursCode);

        // Pour les paiements en attente, on vérifie les devis associés
        // Un paiement est en attente si un Devis existe pour le dossier et que PaiementOk n'est pas à 1 (ou est à 0)
        var pendingPayments = dossiers.Count(d => d.Devis.Any(dev => dev.PaiementOk == 0)); // Assumant 0 = en attente, 1 = payé

        return new DemandesOverviewDto
        {
            Total = total,
            Success = success,
            Failed = failed,
            InProgress = inProgress,
            PendingPayments = pendingPayments
        };
    }
}