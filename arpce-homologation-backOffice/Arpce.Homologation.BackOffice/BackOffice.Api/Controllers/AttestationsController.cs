using BackOffice.Application.Features.Demandes.Commands.UploadCertificat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BackOffice.Api.Controllers;

[ApiController]
[Route("api/attestations")]
[Authorize]
public class AttestationsController : ControllerBase
{
    private readonly ISender _mediator;

    public AttestationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Uploade le fichier PDF signé pour une attestation existante.
    /// </summary>
    /// <param name="attestationId">L'identifiant unique de l'attestation à mettre à jour.</param>
    /// <param name="command">Le contenu du formulaire (fichier, dates).</param>
    [HttpPost("{attestationId:guid}/upload")]
    public async Task<IActionResult> UploadCertificat(Guid attestationId, [FromForm] UploadCertificatCommand command)
    {
        command.AttestationId = attestationId;

        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }
}