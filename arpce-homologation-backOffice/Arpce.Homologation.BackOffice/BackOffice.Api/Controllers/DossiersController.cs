using BackOffice.Application.Features.Demandes.Commands.SetHomologable;
using BackOffice.Application.Features.Dossiers.Commands.RejectDossier;
using BackOffice.Application.Features.Dossiers.Commands.SendMail;
using BackOffice.Application.Features.Dossiers.Commands.ValidateInstruction;
using BackOffice.Application.Features.Dossiers.Queries.GetDossierDetail;
using BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using BackOffice.Application.Features.Dossiers.Queries.GetDossiersOverview;
using BackOffice.Application.Features.Dossiers.Queries.GetMyDossiers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

/// <summary>
/// Contrôleur pour gérer les opérations sur les dossiers d'homologation côté Back Office.
/// </summary>
[ApiController]
[Route("api/dossiers")]
[Authorize] 
public class DossiersController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur.
    /// </summary>
    /// <param name="mediator">L'instance de MediatR pour envoyer les requêtes.</param>
    public DossiersController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère un aperçu statistique global de tous les dossiers dans le système.
    /// </summary>
    /// <returns>Un objet JSON contenant les statistiques agrégées des dossiers.</returns>
    [HttpGet("apercu")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DossiersOverviewDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDossiersOverview()
    {
        // Crée et envoie la requête MediatR au handler correspondant.
        var result = await _mediator.Send(new GetDossiersOverviewQuery());

        // Retourne une réponse 200 OK avec les données.
        return Ok(result);
    }

    /// <summary>
    /// Récupère une liste paginée, filtrée et triée de tous les dossiers du système.
    /// Permet aux agents de rechercher et de consulter l'ensemble des dossiers.
    /// </summary>
    /// <param name="parameters">
    /// </param>
    /// <returns>Un objet contenant la liste des dossiers pour la page demandée et les informations de pagination.</returns>
    [HttpGet] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DossiersListVm))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDossiersList([FromQuery] GetDossiersQueryParameters parameters)
    {
        var query = new GetDossiersListQuery(parameters);

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Récupère la liste des dossiers spécifiquement assignés à l'agent actuellement connecté.
    /// Prend en charge la pagination, la recherche et le tri.
    /// </summary>
    /// <param name="parameters">Les paramètres de pagination, recherche et tri.</param>
    /// <returns>Une liste paginée des dossiers assignés à l'agent.</returns>
    [HttpGet("mes-dossiers")] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DossiersListVm))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyDossiersList([FromQuery] GetDossiersQueryParameters parameters)
    {
        // Crée une instance de la query spécifique pour les dossiers de l'agent.
        var query = new GetMyDossiersQuery(parameters);

        // Envoie la query au handler qui filtrera par l'ID de l'agent connecté.
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Récupère les détails complets d'un dossier spécifique.
    /// </summary>
    /// <param name="dossierId">L'identifiant unique du dossier à récupérer.</param>
    /// <returns>Un objet JSON contenant toutes les informations détaillées du dossier.</returns>
    [HttpGet("{dossierId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DossierDetailVm))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDossierDetail(Guid dossierId)
    {
        try
        {
            var result = await _mediator.Send(new GetDossierDetailQuery(dossierId));
            return Ok(result);
        }
        catch (Exception ex) when (ex.Message.Contains("introuvable"))
        {
            return NotFound(new { title = "Ressource Introuvable", detail = ex.Message, status = 404 });
        }
    }

    /// <summary>
    /// Valide l'instruction d'un dossier, change son statut et ajoute une remarque.
    /// </summary>
    /// <param name="dossierId">L'identifiant du dossier à valider.</param>
    /// <param name="command">L'objet contenant la remarque de validation.</param>
    /// <returns>Une réponse de succès.</returns>
    [HttpPatch("{dossierId:guid}/valider-instruction")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)] 
    public async Task<IActionResult> ValidateInstruction(Guid dossierId, [FromBody] ValidateInstructionCommand command)
    {
        command.DossierId = dossierId;

        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { ok = result });
        }
        catch (InvalidOperationException ex)
        {
            // Gère spécifiquement les erreurs métier (ex: mauvais statut) avec un code 422.
            return UnprocessableEntity(new { title = "Opération Invalide", detail = ex.Message, status = 422 });
        }
        catch (Exception ex) when (ex.Message.Contains("introuvable"))
        {
            // Gère le cas où le dossier n'est pas trouvé avec un code 404.
            return NotFound(new { title = "Ressource Introuvable", detail = ex.Message, status = 404 });
        }
    }

    /// <summary>
    /// Méthode pour le réjé d'une demande(équipement)
    /// </summary>
    /// <param name="dossierId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPatch("{dossierId:guid}/rejeter")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectDossier(Guid dossierId, [FromBody] RejectDossierCommand command)
    {
        command.DossierId = dossierId;
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
    /// Méthode pour l'équipement non homologué
    /// </summary>
    /// <param name="equipementId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPatch("{equipementId:guid}/non-homologable")]
    public async Task<IActionResult> SetHomologable(Guid equipementId, [FromBody] SetEquipementHomologableCommand command)
    {
        command.EquipementId = equipementId;
        return Ok(new { ok = await _mediator.Send(command) });
    }

    [HttpPost("{dossierId:guid}/envoyer-mail")]
    public async Task<IActionResult> SendMail(Guid dossierId, [FromBody] SendMailToClientCommand command)
    {
        command.DossierId = dossierId;
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { ok = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Méthode pour le changement de statuts
    /// </summary>
    /// <param name="dossierId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPatch("{dossierId:guid}/changer-statut")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(Guid dossierId, [FromBody] ChangeDossierStatusCommand command)
    {
        command.DossierId = dossierId;
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { ok = result });
        }
        catch (Exception ex) when (ex.Message.Contains("introuvable"))
        {
            return NotFound(new { title = "Ressource Introuvable", detail = ex.Message, status = 404 });
        }
    }
}