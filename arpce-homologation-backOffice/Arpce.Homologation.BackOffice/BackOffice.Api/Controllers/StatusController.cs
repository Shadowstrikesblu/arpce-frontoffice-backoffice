using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Status.Queries.GetStatusList;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackOffice.Api.Controllers;

[ApiController]
[Route("api/status")]
[Authorize]
public class StatusController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IApplicationDbContext _context;

    public StatusController(ISender mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    /// <summary>
    /// Récupère la liste de tous les statuts (via Mediator).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StatutDto>))]
    public async Task<IActionResult> GetStatusList()
    {
        var result = await _mediator.Send(new GetStatusListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Récupère tous les statuts bruts (Direct DB).
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Statuts.AsNoTracking().ToListAsync());
    }

    /// <summary>
    /// Crée un nouveau statut.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Statut statut)
    {
        if (statut.Id == Guid.Empty) statut.Id = Guid.NewGuid();
        _context.Statuts.Add(statut);
        await _context.SaveChangesAsync(default);
        return Ok(statut);
    }

    /// <summary>
    /// Met à jour un statut existant (Libellé ou Code).
    /// </summary>
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Statut model)
    {
        var s = await _context.Statuts.FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound(new { message = "Statut introuvable" });

        if (!string.IsNullOrEmpty(model.Libelle)) s.Libelle = model.Libelle;
        if (!string.IsNullOrEmpty(model.Code)) s.Code = model.Code;

        await _context.SaveChangesAsync(default);
        return Ok(s);
    }
}