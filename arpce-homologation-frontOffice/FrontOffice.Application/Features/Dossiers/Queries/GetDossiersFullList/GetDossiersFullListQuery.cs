using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList; 
using MediatR;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersFullList;

public class GetDossiersFullListQuery : IRequest<DossiersFullListVm>
{
    public GetDossiersQueryParameters Parameters { get; set; }

    public GetDossiersFullListQuery(GetDossiersQueryParameters parameters)
    {
        Parameters = parameters;
    }
}