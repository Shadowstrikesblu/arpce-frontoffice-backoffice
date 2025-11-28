using MediatR;

namespace BackOffice.Application.Features.Authentication.Queries.CheckToken;

public class CheckTokenQuery : IRequest<AdminUserDto> { }