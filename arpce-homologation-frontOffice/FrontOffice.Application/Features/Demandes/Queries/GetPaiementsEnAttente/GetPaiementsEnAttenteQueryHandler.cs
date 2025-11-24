using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Demandes.Queries.GetPaiementsEnAttente;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Demandes.Queries.GetPaiementEnAttente;

/// <summary>
/// Gère la logique de la requête pour obtenir le paiement en attente d'un dossier spécifique.
/// </summary>
public class GetPaiementEnAttenteByDossierQueryHandler : IRequestHandler<GetPaiementEnAttenteByDossierQuery, PaiementEnAttenteDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public GetPaiementEnAttenteByDossierQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la logique pour trouver le paiement en attente.
    /// </summary>
    public async Task<PaiementEnAttenteDto?> Handle(GetPaiementEnAttenteByDossierQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Recherche du premier devis non payé pour le dossier spécifié,
        var paiementEnAttente = await _context.Dossiers
            .Where(d => d.Id == request.DossierId && d.IdClient == userId.Value)
            .Include(d => d.ModeReglement)
            .SelectMany(d => d.Devis
                .Where(devis => devis.PaiementOk == 0) 
                .Select(devis => new PaiementEnAttenteDto
                {
                    Id = d.Id,
                    NumeroDemande = d.Numero,
                    Montant = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0), // Montant total du devis
                    DateEcheance = devis.Date,
                    //ModeReglementLibelle = d.ModeReglement.Libelle
                })
            )
            .FirstOrDefaultAsync(cancellationToken); 

        return paiementEnAttente;
    }
}