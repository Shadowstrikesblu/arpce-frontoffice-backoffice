using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using MediatR;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersDevisNonValides;

public class GetDossiersDevisNonValidesQuery : IRequest<DossiersListVm> 
{
    public GetDossiersQueryParameters Parameters { get; set; }

    public GetDossiersDevisNonValidesQuery(GetDossiersQueryParameters parameters)
    {
        Parameters = parameters;
    }
}