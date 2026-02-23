using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Features.Signataires.Commands.CreateSignataire;
using BackOffice.Application.Features.Signataires.Commands.DeleteSignataire;
using BackOffice.Application.Features.Signataires.Commands.UpdateSignataire;
using BackOffice.Application.Features.Signataires.Queries.GetSignataireById;
using BackOffice.Application.Features.Signataires.Queries.GetSignatairesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Accessible au BO authentifié
public class SignatairesController : ControllerBase
{
    private readonly ISender _mediator;

    public SignatairesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Liste tous les signataires.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SignatairesListVm>> GetAll([FromQuery] GetSignatairesListQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    /// <summary>
    /// Récupère un signataire par son identifiant unique.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SignataireDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSignataireByIdQuery { Id = id });
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Crée un nouveau signataire avec son image de signature.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Guid>> Create([FromForm] CreateSignataireCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Met à jour les informations d'un signataire (y compris l'image si fournie).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<bool>> Update(Guid id, [FromForm] UpdateSignataireCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Supprime un signataire.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteSignataireCommand { Id = id });
        if (!result) return NotFound();
        return Ok(result);
    }
}