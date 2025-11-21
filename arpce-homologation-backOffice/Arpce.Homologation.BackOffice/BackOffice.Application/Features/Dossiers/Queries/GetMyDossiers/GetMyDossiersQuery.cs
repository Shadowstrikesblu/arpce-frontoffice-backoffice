using BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using MediatR;

namespace BackOffice.Application.Features.Dossiers.Queries.GetMyDossiers;

/// <summary>
/// Requête MediatR pour obtenir la liste des dossiers assignés à l'agent actuellement connecté.
/// Les paramètres de recherche, tri et pagination sont identiques à ceux de la liste globale.
/// </summary>
public class GetMyDossiersQuery : IRequest<DossiersListVm>
{
    public GetDossiersQueryParameters Parameters { get; set; }

    public GetMyDossiersQuery(GetDossiersQueryParameters parameters)
    {
        Parameters = parameters;
    }
}