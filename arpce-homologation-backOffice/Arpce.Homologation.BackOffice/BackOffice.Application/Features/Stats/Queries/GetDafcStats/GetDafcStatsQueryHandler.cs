using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Stats.Queries.GetDafcStats;

public class GetDafcStatsQueryHandler : IRequestHandler<GetDafcStatsQuery, DafcStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetDafcStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DafcStatsDto> Handle(GetDafcStatsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Dossiers.AsNoTracking();

        var nbFactureExpire = await query.CountAsync(d => d.Statut.Code == StatutDossierEnum.PaiementExpirer.ToString(), cancellationToken);

        var nbFactureEnAttente = await query.CountAsync(d => d.Statut.Code == StatutDossierEnum.DevisPaiement.ToString(), cancellationToken);

        var nbFacturePaye = await query.CountAsync(d => d.Statut.Code == StatutDossierEnum.DossierPayer.ToString(), cancellationToken);

        var nbDevisValide = await query.CountAsync(d => d.Statut.Code == StatutDossierEnum.DevisValide.ToString(), cancellationToken);

        var nbDevisRefuse = await query.CountAsync(d => d.Statut.Code == StatutDossierEnum.DevisRefuser.ToString(), cancellationToken);

        var nbDevisEnAttente = await query.CountAsync(d => d.Statut.Code == StatutDossierEnum.DevisEmit.ToString(), cancellationToken);

        return new DafcStatsDto
        {
            NbFactureExpire = nbFactureExpire,
            NbFactureEnAttente = nbFactureEnAttente,
            NbFacturePaye = nbFacturePaye,
            NbDevisValide = nbDevisValide,
            NbDevisRefuse = nbDevisRefuse,
            NbDevisEnAttente = nbDevisEnAttente
        };
    }
}