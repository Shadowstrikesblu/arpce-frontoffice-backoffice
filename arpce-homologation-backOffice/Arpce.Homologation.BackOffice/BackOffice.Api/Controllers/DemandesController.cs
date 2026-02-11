using BackOffice.Application.Features.Demandes.Commands.AddCategorieToDemande;
using BackOffice.Application.Features.Demandes.Commands.ChangeStatus;
using BackOffice.Application.Features.Demandes.Commands.RejectEquipement;
using BackOffice.Application.Features.Demandes.Commands.UpdateEquipement;
using BackOffice.Application.Features.Demandes.Commands.UploadCertificat;
using BackOffice.Application.Features.Demandes.Queries.DownloadDocument;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

/// <summary>
/// Contrôleur pour gérer les actions spécifiques aux demandes (équipements) côté Back Office.
/// </summary>
[ApiController]
[Route("api/demandes")] 
[Authorize]
public class DemandesController : ControllerBase
{
    private readonly ISender _mediator;

    public DemandesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Assigne une catégorie d'équipement à une demande (équipement) spécifique.
    /// Cette action est généralement effectuée par un agent de la DRSE.
    /// </summary>
    /// <param name="demandeId">L'identifiant de la demande (équipement) à mettre à jour.</param>
    /// <param name="command">L'objet contenant l'ID de la catégorie à assigner.</param>
    /// <returns>Une réponse de succès.</returns>
    [HttpPost("{demandeId:guid}/categorie")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCategorieToDemande(Guid demandeId, [FromBody] AddCategorieToDemandeCommand command)
    {
        // On s'assure que l'ID de la route correspond à celui de la commande.
        command.DemandeId = demandeId;

        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { ok = result });
        }
        catch (Exception ex) when (ex.Message.Contains("introuvable"))
        {
            // Retourne une 404 claire si la demande ou la catégorie n'existe pas.
            return NotFound(new { title = "Ressource Introuvable", detail = ex.Message, status = 404 });
        }
    }

   
    /// <summary>
    /// Pour télé charger le document
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{type}/{id:guid}/download")]
    public async Task<IActionResult> DownloadDocument(string type, Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DownloadDocumentQuery(id, type));
            return File(result.FileContents, result.ContentType, result.FileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// La récupération de l'équipement by id
    /// </summary>
    /// <param name="equipementId"></param>
    /// <param name="command"></param>Ok
    /// <returns></returns>
    [HttpPatch("{equipementId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateEquipement(Guid equipementId, [FromBody] UpdateEquipementCommand command)
    {
        command.EquipementId = equipementId;
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Refuse un équipement spécifique 
    /// </summary>
    /// <param name="equipement">L'identifiant unique de l'équipement à rejeter.</param>
    /// <param name="command">Les informations sur le motif du rejet.</param>
    [HttpPatch("{equipement:guid}/rejeter")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectEquipement(Guid equipement, [FromBody] RejectEquipementCommand command)
    {
        command.EquipementId = equipement;

        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { ok = result });
        }
        catch (Exception ex)
        {
            return NotFound(new { title = "Erreur", detail = ex.Message, status = 404 });
        }
    }

    /// <summary>
    /// Change le statut d'un équipement spécifique 
    /// </summary>
    [HttpPatch("{equipement:guid}/changer-statut")]
    public async Task<IActionResult> ChangeEquipementStatus(Guid equipement, [FromBody] ChangeEquipementStatusCommand command)
    {
        command.EquipementId = equipement;
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }
}