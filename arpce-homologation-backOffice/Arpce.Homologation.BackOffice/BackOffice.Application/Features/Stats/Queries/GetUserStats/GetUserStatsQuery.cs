using MediatR;

namespace BackOffice.Application.Features.Stats.Queries.GetUserStats;

public class GetUserStatsQuery : IRequest<UserStatsDto> { }