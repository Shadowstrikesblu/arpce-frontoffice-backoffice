using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using MediatR;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetFacturesNonValidees;

public class GetFacturesNonValideesQuery : IRequest<DossiersListVm>
{
    public GetDossiersQueryParameters Parameters { get; set; }

    public GetFacturesNonValideesQuery(GetDossiersQueryParameters parameters)
    {
        Parameters = parameters;
    }
}