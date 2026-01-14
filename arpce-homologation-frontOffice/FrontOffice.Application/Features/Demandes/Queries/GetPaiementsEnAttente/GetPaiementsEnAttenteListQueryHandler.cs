using FrontOffice.Application.Common.Interfaces;
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
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException();
        }

        // On cible spécifiquement le code de statut "EnPaiement"
        const string statutPaiementCode = "EnPaiement";

        // Requête pour trouver les dossiers du client qui ont le bon statut
        // ET qui ont au moins un devis non payé.
        var dossiersEnAttente = await _context.Dossiers
            .AsNoTracking()
            .Where(d => d.IdClient == userId.Value && d.Statut.Code == statutPaiementCode)
            .Include(d => d.Devis) // On inclut les devis pour les lire
            .ToListAsync(cancellationToken);

        // On transforme les résultats en DTO en mémoire.
        // C'est plus facile à déboguer que de tout faire dans une seule grosse requête LINQ.
        var paiements = new List<PaiementEnAttenteDto>();

        foreach (var dossier in dossiersEnAttente)
        {
            // On cherche le premier devis non payé (PaiementOk est 0 ou NULL)
            var devisAPayer = dossier.Devis
                .FirstOrDefault(devis => devis.PaiementOk == 0 || devis.PaiementOk == null);

            if (devisAPayer != null)
            {
                paiements.Add(new PaiementEnAttenteDto
                {
                    Id = dossier.Id,
                    NumeroDossier = dossier.Numero,
                    Montant = devisAPayer.MontantEtude + (devisAPayer.MontantHomologation ?? 0) + (devisAPayer.MontantControle ?? 0),
                    DateEcheance = devisAPayer.Date
                });
            }
        }

        return paiements;
    }
}