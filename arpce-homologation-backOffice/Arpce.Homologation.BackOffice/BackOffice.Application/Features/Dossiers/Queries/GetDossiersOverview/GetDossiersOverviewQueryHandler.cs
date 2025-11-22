using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersOverview;

public class GetDossiersOverviewQueryHandler : IRequestHandler<GetDossiersOverviewQuery, DossiersOverviewDto>
{
    private readonly IApplicationDbContext _context;

    public GetDossiersOverviewQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DossiersOverviewDto> Handle(GetDossiersOverviewQuery request, CancellationToken cancellationToken)
    {
        // Définition des codes de statut.
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

        var allDossiersQuery = _context.Dossiers.AsNoTracking();

        var total = await allDossiersQuery.CountAsync(cancellationToken);

        var success = await allDossiersQuery
            .CountAsync(d => d.Statut.Code == statutSuccessCode, cancellationToken);

        var failed = await allDossiersQuery
            .CountAsync(d => d.Statut.Code == statutFailedCode, cancellationToken);

        var inProgress = await allDossiersQuery
            .CountAsync(d => statutInProgressCodes.Contains(d.Statut.Code), cancellationToken);

        return new DossiersOverviewDto
        {
            Total = total,
            Success = success,
            Failed = failed,
            InProgress = inProgress
        };
    }
}