using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Enums; 
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersOverview;

/// <summary>
/// Gère la logique de la requête pour obtenir l'aperçu statistique global des dossiers.
/// Ce handler lit les données de la base de données du Back Office.
/// </summary>
public class GetDossiersOverviewQueryHandler : IRequestHandler<GetDossiersOverviewQuery, DossiersOverviewDto>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    /// <param name="context">Le contexte de la base de données du Back Office.</param>
    public GetDossiersOverviewQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Exécute la logique pour obtenir l'aperçu global des dossiers.
    /// </summary>
    /// <param name="request">La requête MediatR (sans paramètres).</param>
    /// <param name="cancellationToken">Le token d'annulation.</param>
    /// <returns>Un DTO contenant les statistiques d'aperçu.</returns>
    public async Task<DossiersOverviewDto> Handle(GetDossiersOverviewQuery request, CancellationToken cancellationToken)
    {
        // Défini les codes de statut pour chaque catégorie de calcul.
        var statutSuccessCode = StatutDossierEnum.ApprouveAttestationSignee.ToString();
        var statutFailedCode = StatutDossierEnum.Rejetee.ToString();
        var statutInProgressCodes = new[]
        {
            StatutDossierEnum.NouvelleDemande.ToString(),
            StatutDossierEnum.EnCoursInstruction.ToString(),
            StatutDossierEnum.EnvoyePourApprobation.ToString(),
            StatutDossierEnum.ApprouveAttentePaiement.ToString(),
            StatutDossierEnum.ApprouvePaiementEffectue.ToString()
        };

        // Exécute les requêtes de comptage de manière asynchrone.
        var allDossiersQuery = _context.Dossiers.AsNoTracking();

        var totalTask = allDossiersQuery.CountAsync(cancellationToken);

        var successTask = allDossiersQuery
            .CountAsync(d => d.Statut.Code == statutSuccessCode, cancellationToken);

        var failedTask = allDossiersQuery
            .CountAsync(d => d.Statut.Code == statutFailedCode, cancellationToken);

        var inProgressTask = allDossiersQuery
            .CountAsync(d => statutInProgressCodes.Contains(d.Statut.Code), cancellationToken);

        // Attendre que toutes les requêtes de comptage se terminent.
        await Task.WhenAll(totalTask, successTask, failedTask, inProgressTask);

        // Construire et retourner le DTO de réponse.
        return new DossiersOverviewDto
        {
            Total = totalTask.Result,
            Success = successTask.Result,
            Failed = failedTask.Result,
            InProgress = inProgressTask.Result
        };
    }
}