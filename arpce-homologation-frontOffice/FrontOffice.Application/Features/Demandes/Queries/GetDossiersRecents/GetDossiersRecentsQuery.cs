using MediatR;

namespace FrontOffice.Application.Features.Demandes.Queries.GetDossiersRecents;

/// <summary>
/// Requête pour obtenir la liste des dossiers récents pour le client connecté.
/// </summary>
public class GetDossiersRecentsQuery : IRequest<List<DossierRecentItemDto>>
{

}