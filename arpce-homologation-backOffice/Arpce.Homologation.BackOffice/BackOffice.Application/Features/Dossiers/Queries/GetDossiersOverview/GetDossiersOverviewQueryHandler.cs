using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        var allDossiersQuery = _context.Dossiers.AsNoTracking();

        var statutSuccessCode = StatutDossierEnum.DossierSigne.ToString();

        var statutsFailedCodes = new[]
        {
            StatutDossierEnum.RefusDossier.ToString(),   // Refus initial ou en cours
            StatutDossierEnum.DevisRefuser.ToString(),   // Refus du devis par le client
            StatutDossierEnum.PaiementRejete.ToString(), // Paiement invalide
            StatutDossierEnum.PaiementExpirer.ToString() // Délai dépassé
        };

        var statutsInProgressCodes = new[]
        {
            StatutDossierEnum.NouveauDossier.ToString(),
            StatutDossierEnum.Instruction.ToString(),
            StatutDossierEnum.ApprobationInstruction.ToString(),
            StatutDossierEnum.InstructionApprouve.ToString(),
            StatutDossierEnum.DevisCreer.ToString(),
            StatutDossierEnum.DevisValideSC.ToString(),
            StatutDossierEnum.DevisValideTr.ToString(),
            StatutDossierEnum.DevisEmit.ToString(),
            StatutDossierEnum.DevisValide.ToString(),
            StatutDossierEnum.DevisPaiement.ToString(),
            StatutDossierEnum.DossierPayer.ToString(),
            StatutDossierEnum.DossierSignature.ToString()
        };

        // Exécution séquentielle des requêtes de comptage
        var total = await allDossiersQuery.CountAsync(cancellationToken);

        var success = await allDossiersQuery
            .CountAsync(d => d.Statut.Code == statutSuccessCode, cancellationToken);

        var failed = await allDossiersQuery
            .CountAsync(d => statutsFailedCodes.Contains(d.Statut.Code), cancellationToken);

        var inProgress = await allDossiersQuery
            .CountAsync(d => d.Statut != null && statutsInProgressCodes.Contains(d.Statut.Code), cancellationToken);

        return new DossiersOverviewDto
        {
            Total = total,
            Success = success,
            Failed = failed,
            InProgress = inProgress
        };
    }
}