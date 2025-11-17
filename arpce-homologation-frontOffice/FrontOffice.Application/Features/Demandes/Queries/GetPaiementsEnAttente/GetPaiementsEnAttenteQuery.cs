using FrontOffice.Application.Features.Demandes.Queries.GetPaiementsEnAttente;
using MediatR;

namespace FrontOffice.Application.Features.Demandes.Queries.GetPaiementEnAttente;

/// <summary>
/// Requête pour obtenir le prochain paiement en attente pour un dossier spécifique.
/// </summary>
public class GetPaiementEnAttenteByDossierQuery : IRequest<PaiementEnAttenteDto?> 
{
    /// <summary>
    /// L'identifiant unique du dossier pour lequel on cherche un paiement.
    /// </summary>
    public Guid DossierId { get; set; }

    public GetPaiementEnAttenteByDossierQuery(Guid dossierId)
    {
        DossierId = dossierId;
    }
}