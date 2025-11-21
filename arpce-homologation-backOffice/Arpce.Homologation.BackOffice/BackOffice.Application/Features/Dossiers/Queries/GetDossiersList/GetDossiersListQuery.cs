using MediatR;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Requête MediatR pour obtenir la liste complète des dossiers avec filtres, tri et pagination.
/// </summary>
public class GetDossiersListQuery : IRequest<DossiersListVm>
{
    public GetDossiersQueryParameters Parameters { get; set; }

    public GetDossiersListQuery(GetDossiersQueryParameters parameters)
    {
        Parameters = parameters;
    }
}