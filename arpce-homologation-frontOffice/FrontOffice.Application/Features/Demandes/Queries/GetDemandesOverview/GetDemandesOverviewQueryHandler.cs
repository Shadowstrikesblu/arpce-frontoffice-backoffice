using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Demandes.Queries.GetDemandesOverview;

public class GetDemandesOverviewQueryHandler : IRequestHandler<GetDemandesOverviewQuery, DemandesOverviewDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDemandesOverviewQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DemandesOverviewDto> Handle(GetDemandesOverviewQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var dossiers = await _context.Dossiers
            .Where(d => d.IdClient == userId.Value)
            .Include(d => d.Demande) 
            .Include(d => d.Statut)
            .Include(d => d.Devis)
            .ToListAsync(cancellationToken);

        var statutValideCode = "VALID";
        var statutRejeteCode = "REJET";
        var statutEnCoursCode = "ENCRS";

        var total = dossiers.Count;
        var success = dossiers.Count(d => d.Statut?.Code == statutValideCode);
        var failed = dossiers.Count(d => d.Statut?.Code == statutRejeteCode);
        var inProgress = dossiers.Count(d => d.Statut?.Code == statutEnCoursCode);

        var pendingPayments = dossiers.Count(d => d.Devis.Any(dev => dev.PaiementOk == 0));

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