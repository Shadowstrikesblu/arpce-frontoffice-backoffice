using BackOffice.Application.Features.Categories.Commands.CreateCategorie;
using BackOffice.Application.Features.Categories.Commands.DeleteCategorie;
using BackOffice.Application.Features.Categories.Commands.UpdateCategorie;
using BackOffice.Application.Features.Categories.Queries.GetCategoriesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

/// <summary>
/// Contrôleur pour gérer les opérations CRUD sur les Catégories d'Équipement.
/// </summary>
[ApiController]
[Route("api/categories-equipement")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ISender _mediator;

    public CategoriesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Récupère la liste des catégories d'équipement, avec un filtre optionnel par type d'équipement.
    /// </summary>
    /// <param name="typeEquipement">Paramètre de requête optionnel pour filtrer par type.</param>
    /// <returns>Une liste de catégories.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategorieEquipementDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategoriesList([FromQuery] string? typeEquipement)
    {
        var query = new GetCategoriesListQuery { TypeEquipement = typeEquipement };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CategorieEquipementDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCategorie([FromBody] CreateCategorieCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategoriesList), new { typeEquipement = result.TypeEquipement }, result);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategorie([FromBody] UpdateCategorieCommand command)
    {
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

    [HttpDelete("{categorieId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategorie(Guid categorieId)
    {
        try
        {
            var result = await _mediator.Send(new DeleteCategorieCommand(categorieId));
            return Ok(new { ok = result });
        }
        catch (Exception ex) when (ex.Message.Contains("introuvable"))
        {
            return NotFound(new { title = "Ressource Introuvable", detail = ex.Message, status = 404 });
        }
    }
}