using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList
{
    public class GetProfilsListQuery : IRequest<List<ProfilListItemDto>>
    {
    }
}
