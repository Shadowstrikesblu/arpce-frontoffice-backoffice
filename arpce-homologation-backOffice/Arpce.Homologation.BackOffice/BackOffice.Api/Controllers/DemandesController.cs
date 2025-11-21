using BackOffice.Application.Features.Demandes.Commands.AddCategorieToDemande;
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
}