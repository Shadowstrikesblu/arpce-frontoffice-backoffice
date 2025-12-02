// Fichier : FrontOffice.Application/Features/Demandes/Queries/GetPaiementsEnAttente/GetPaiementsEnAttenteListQueryHandler.cs

using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Le statut qui indique que le dossier attend un paiement du client.
        // Dans la nouvelle liste, c'est "DevisPaiement" (Approuvé, en attente de paiement).
        // On pourrait aussi inclure "DevisEmit" si le paiement est possible dès l'émission.
        var statutPaiementCode = StatutDossierEnum.DevisPaiement.ToString();

        var paiements = await _context.Dossiers
            .AsNoTracking()
            // On filtre sur le client ET sur le statut "En attente de paiement"
            .Where(d => d.IdClient == userId.Value && d.Statut.Code == statutPaiementCode)
            .Include(d => d.Devis) // Nécessaire pour calculer le montant
            .Select(d => new PaiementEnAttenteDto
            {
                Id = d.Id,
                NumeroDossier = d.Numero,

                // Calcul du montant total à payer : somme des devis non payés (PaiementOk != 1).
                // Avec le nouveau modèle Devis, le montant est la somme des lignes (si implémenté) ou des champs (si conservés).
                // Ici, on utilise les champs existants de l'entité Devis.
                Montant = d.Devis
                    .Where(devis => devis.PaiementOk != 1)
                    .Sum(devis => devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0)),

                // Date d'échéance du dernier devis
                DateEcheance = d.Devis
                    .Where(devis => devis.PaiementOk != 1)
                    .OrderByDescending(devis => devis.Date)
                    .Select(devis => devis.Date)
                    .FirstOrDefault()
            })
            .Where(dto => dto.Montant > 0) // On ne retourne que s'il y a quelque chose à payer
            .ToListAsync(cancellationToken);

        return paiements;
    }
}