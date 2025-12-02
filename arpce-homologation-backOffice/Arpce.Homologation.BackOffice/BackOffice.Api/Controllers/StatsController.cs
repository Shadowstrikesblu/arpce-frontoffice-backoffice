using BackOffice.Application.Features.Stats.Queries.GetDafcStats;
using BackOffice.Application.Features.Stats.Queries.GetUserStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

[ApiController]
[Route("api/stats")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly ISender _mediator;

    public StatsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("utilisateurs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserStatsDto))]
    public async Task<IActionResult> GetUserStats()
    {
        var result = await _mediator.Send(new GetUserStatsQuery());
        return Ok(result);
    }

    [HttpGet("dafc")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DafcStatsDto))]
    public async Task<IActionResult> GetDafcStats()
    {
        var result = await _mediator.Send(new GetDafcStatsQuery());
        return Ok(result);
    }
}