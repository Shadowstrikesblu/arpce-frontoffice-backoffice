using BackOffice.Application.Features.Authentication.Queries.CheckToken; 
using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetAccessList;

public class GetAccessListQuery : IRequest<List<AdminAccessDto>> { }