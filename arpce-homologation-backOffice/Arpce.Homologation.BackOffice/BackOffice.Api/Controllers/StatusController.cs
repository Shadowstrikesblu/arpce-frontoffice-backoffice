using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Features.Status.Queries.GetStatusList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

/// <summary>
/// Contrôleur pour gérer les ressources liées aux statuts des dossiers.
/// </summary>
[ApiController]
[Route("api/status")] 
[Authorize] 
public class StatusController : ControllerBase
{
    private readonly ISender _mediator;

    public StatusController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère la liste de tous les statuts disponibles dans le système.
    /// </summary>
    /// <returns>Une liste d'objets StatutDto.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StatutDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStatusList()
    {
        var query = new GetStatusListQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}