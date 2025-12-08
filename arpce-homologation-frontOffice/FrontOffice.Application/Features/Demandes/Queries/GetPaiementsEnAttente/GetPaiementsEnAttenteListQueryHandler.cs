using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Demandes.Queries.GetPaiementsEnAttente;

public class GetPaiementsEnAttenteListQueryHandler : IRequestHandler<GetPaiementsEnAttenteListQuery, List<PaiementEnAttenteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetPaiementsEnAttenteListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<PaiementEnAttenteDto>> Handle(GetPaiementsEnAttenteListQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException();

        var statutPaiementCode = StatutDossierEnum.DevisPaiement.ToString();

        // On change la requête pour être plus explicite
        var paiements = await _context.Dossiers
            .AsNoTracking()
            .Where(d => d.IdClient == userId.Value && d.Statut.Code == statutPaiementCode)
            .Select(d => new // Projection intermédiaire
            {
                d.Id,
                d.Numero,
                // On calcule le montant et la date directement dans la projection
                MontantAPayer = d.Demandes
                    .SelectMany(dem => dem.Devis) // On prend tous les devis de toutes les demandes du dossier
                    .Where(devis => devis.PaiementOk != 1)
                    .Sum(devis => devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0)),

                DerniereEcheance = d.Demandes
                    .SelectMany(dem => dem.Devis)
                    .Where(devis => devis.PaiementOk != 1)
                    .OrderByDescending(devis => devis.Date)
                    .Select(devis => (DateTime?)devis.Date) // Cast en nullable
                    .FirstOrDefault()
            })
            .Where(x => x.MontantAPayer > 0 && x.DerniereEcheance.HasValue) // On filtre après le calcul
            .Select(x => new PaiementEnAttenteDto
            {
                Id = x.Id,
                NumeroDossier = x.Numero,
                Montant = x.MontantAPayer,
                DateEcheance = x.DerniereEcheance.Value
            })
            .ToListAsync(cancellationToken);

        return paiements;
    }
}