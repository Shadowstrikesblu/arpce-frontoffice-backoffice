using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums; 
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Demandes.Queries.GetPaiementsEnAttente;

/// <summary>
/// Gère la logique de récupération de la liste des paiements en attente pour un client.
/// </summary>
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
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Le code du statut "En attente de paiement"
        var statutPaiementCode = StatutDossierEnum.DevisPaiement.ToString();

        // On récupère les dossiers du client qui sont au statut "En attente de paiement"
        var paiements = await _context.Dossiers
            .AsNoTracking()
            .Where(d => d.IdClient == userId.Value && d.Statut.Code == statutPaiementCode)
            .Include(d => d.Devis) 
            .Select(d => new PaiementEnAttenteDto
            {
                Id = d.Id, // ID du Dossier
                NumeroDossier = d.Numero,

                // Calcul du montant total à payer.
                // On prend la somme des montants du/des devis associés qui ne sont pas encore payés (PaiementOk != 1).
                Montant = d.Devis
                    .Where(devis => devis.PaiementOk != 1)
                    .Sum(devis => devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0)),

                // On prend la date d'échéance du devis le plus récent
                DateEcheance = d.Devis
                    .Where(devis => devis.PaiementOk != 1)
                    .OrderByDescending(devis => devis.Date)
                    .Select(devis => devis.Date)
                    .FirstOrDefault()
            })
            // Filtre final pour ne retourner que ceux qui ont effectivement un montant > 0 calculé
            .Where(dto => dto.Montant > 0)
            .ToListAsync(cancellationToken);

        return paiements;
    }
}