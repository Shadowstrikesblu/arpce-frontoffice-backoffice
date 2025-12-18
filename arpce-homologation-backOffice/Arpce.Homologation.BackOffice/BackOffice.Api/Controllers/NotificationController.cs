using BackOffice.Application.Features.Notifications.Queries.GetNotificationHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly ISender _mediator;

    public NotificationController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère l'historique des notifications pour l'utilisateur connecté (Personnelles + Groupe).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetHistory([FromQuery] GetNotificationHistoryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}