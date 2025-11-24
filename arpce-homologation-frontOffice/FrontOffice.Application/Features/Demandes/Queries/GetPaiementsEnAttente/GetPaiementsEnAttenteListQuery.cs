using MediatR;

namespace FrontOffice.Application.Features.Demandes.Queries.GetPaiementsEnAttente;

/// <summary>
/// Requête MediatR pour obtenir la liste de tous les paiements en attente 
/// pour le client actuellement connecté (tous dossiers confondus).
/// </summary>
public class GetPaiementsEnAttenteListQuery : IRequest<List<PaiementEnAttenteDto>>
{
}