using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Enums;
using FrontOffice.Domain.Enums;
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
        var allDossiersQuery = _context.Dossiers.AsNoTracking();

        // Succès : "DossierSigne" (Attestation signée, le processus est fini avec succès)
        var statutSuccessCode = StatutDossierEnum.DossierSigne.ToString();

        // Échec : "DevisRejete" ou "PaiementRejete" (On peut aussi inclure "PaiementExpire" si considéré comme échec définitif)
        var statutsFailedCodes = new[]
        {
            StatutDossierEnum.DevisRejete.ToString(),
            StatutDossierEnum.PaiementRejete.ToString(),
            StatutDossierEnum.PaiementExpire.ToString()
        };

        // En cours : Tout le reste
        var statutsInProgressCodes = new[]
        {
            StatutDossierEnum.NouveauDossier.ToString(),
            StatutDossierEnum.Instruction.ToString(),
            StatutDossierEnum.ApprobationInstruction.ToString(),
            StatutDossierEnum.InstructionApprouve.ToString(),
            StatutDossierEnum.DevisEmis.ToString(),
            StatutDossierEnum.DevisValide.ToString(),
            StatutDossierEnum.DevisPaiement.ToString(),
            StatutDossierEnum.DossierPaye.ToString(),
            StatutDossierEnum.DossierSignature.ToString()
        };

        // Requêtes séquentielles
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