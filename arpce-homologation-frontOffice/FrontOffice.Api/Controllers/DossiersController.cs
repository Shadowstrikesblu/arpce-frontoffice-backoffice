using FrontOffice.Application.Features.Demandes.Commands.AddEquipementToDossier;
using FrontOffice.Application.Features.Demandes.Commands.CreateDossier;
using FrontOffice.Application.Features.Demandes.Queries.GetDemandesOverview;
using FrontOffice.Application.Features.Demandes.Queries.GetDossiersRecents;
using FrontOffice.Application.Features.Demandes.Queries.GetPaiementEnAttente;
using FrontOffice.Application.Features.Demandes.Queries.GetPaiementsEnAttente;
using FrontOffice.Application.Features.Dossiers.Commands.ValidateDevis;
using FrontOffice.Application.Features.Dossiers.Queries.GetDossierDetail;
using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersDevisNonValides;
using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using FrontOffice.Application.Features.Dossiers.Queries.GetFacturesNonValidees;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Api.Controllers;

/// <summary>
/// Contrôleur principal pour la gestion des Dossiers d'homologation.
/// Toutes les actions ici sont centrées sur la ressource "Dossier".
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
    public DossiersController(ISender mediator)
    {
        _mediator = mediator;
    }

    // --- ENDPOINTS DE CONSULTATION (GET) ---

    /// <summary>
    /// Récupère un aperçu chiffré des dossiers pour le tableau de bord.
    /// </summary>
    [HttpGet("apercu")] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DemandesOverviewDto))]
    public async Task<IActionResult> GetDossiersOverview()
    {
        var result = await _mediator.Send(new GetDemandesOverviewQuery());
        return Ok(result);
    }

    /// <summary>
    /// Récupère le prochain paiement dû pour un dossier spécifique.
    /// Si aucun paiement n'est en attente pour ce dossier, la réponse sera 404 Not Found.
    /// </summary>
    /// <param name="dossierId">L'identifiant du dossier à vérifier.</param>
    /// <returns>Les détails du paiement en attente.</returns>
    //[HttpGet("{dossierId:guid}/paiement-en-attente")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaiementEnAttenteDto))]
    //[ProducesResponseType(StatusCodes.Status404NotFound)] 
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> GetPaiementEnAttenteForDossier(Guid dossierId)
    //{
    //    var query = new GetPaiementEnAttenteByDossierQuery(dossierId);
    //    var result = await _mediator.Send(query);

    //    if (result == null)
    //    {
    //        return NotFound(new { message = "Aucun paiement en attente trouvé pour ce dossier." });
    //    }

    //    return Ok(result);
    //}


    /// <summary>
    /// Récupère la liste de tous les paiements en attente pour le client connecté (tous dossiers confondus).
    /// </summary>
    [HttpGet("paiements-en-attente")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PaiementEnAttenteDto>))]
    public async Task<IActionResult> GetPaiementsEnAttenteList()
    {
        // Utilise la Query qui retourne une liste
        var result = await _mediator.Send(new GetPaiementsEnAttenteListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Récupère la liste des dossiers créés récemment par l'utilisateur.
    /// </summary>
    [HttpGet("recents")] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DossierRecentItemDto>))]
    public async Task<IActionResult> GetDossiersRecents()
    {
        var result = await _mediator.Send(new GetDossiersRecentsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Récupère une liste paginée, filtrée et triée des dossiers de l'utilisateur.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DossiersListVm))]
    public async Task<IActionResult> GetDossiersList([FromQuery] GetDossiersQueryParameters parameters)
    {
        var result = await _mediator.Send(new GetDossiersListQuery(parameters));
        return Ok(result);
    }

    /// <summary>
    /// Récupère les détails complets d'un dossier spécifique.
    /// </summary>
    [HttpGet("{dossierId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DossierDetailVm))]
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
            return NotFound(new { error = ex.Message });
        }
    }

    // --- ENDPOINTS DE CRÉATION/MODIFICATION (POST) ---

    /// <summary>
    /// MODIFIÉ : Crée un nouveau dossier d'homologation.
    /// </summary>
    [HttpPost] 
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateDossierResponseDto))]
    public async Task<IActionResult> CreateDossier([FromForm] CreateDossierCommand command)
    {
        var result = await _mediator.Send(command);
        // La réponse pointe vers le nouvel endpoint de détail
        return CreatedAtAction(nameof(GetDossierDetail), new { dossierId = result.DossierId }, result);
    }

    /// <summary>
    /// MODIFIÉ : Ajoute un équipement (demande) à un dossier existant.
    /// </summary>
    [HttpPost("{dossierId:guid}/equipements")] 
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> AddEquipementToDossier(Guid dossierId, [FromForm] AddEquipementToDossierCommand command)
    {
        // On s'assure que l'ID de la route correspond à celui de la commande pour la cohérence
        command.IdDossier = dossierId;
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }


    /// <summary>
    /// Pour les devis non validés
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("devis-non-valides")]
    public async Task<IActionResult> GetDevisNonValides([FromQuery] GetDossiersQueryParameters parameters)
    {
        var result = await _mediator.Send(new GetDossiersDevisNonValidesQuery(parameters));
        return Ok(result);
    }

    /// <summary>
    /// Récupération des factures non validées
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("factures-non-validees")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DossiersListVm))]
    public async Task<IActionResult> GetFacturesNonValidees([FromQuery] GetDossiersQueryParameters parameters)
    {
        var result = await _mediator.Send(new GetFacturesNonValideesQuery(parameters));
        return Ok(result);
    }

    /// <summary>
    /// Permet au client de valider le devis qui lui a été soumis.
    /// </summary>
    [HttpPost("{dossierId:guid}/valider-devis")]
    [Authorize] // Nécessite d'être connecté
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)] // Si le dossier n'est pas au bon statut
    public async Task<IActionResult> ValidateDevis(Guid dossierId)
    {
        var command = new ValidateDevisCommand { DossierId = dossierId };
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }
}