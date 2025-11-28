using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetUserTypes
{
    public class GetAdminUserTypesQuery : IRequest<List<AdminUserTypeDto>> { }
}
