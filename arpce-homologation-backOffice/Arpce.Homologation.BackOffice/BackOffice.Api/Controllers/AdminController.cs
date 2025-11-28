using BackOffice.Application.Features.Admin.Commands.AssignProfilToAdmin;
using BackOffice.Application.Features.Admin.Commands.AssignProfilToLdapUser;
using BackOffice.Application.Features.Admin.Commands.ChangeAdminPassword;
using BackOffice.Application.Features.Admin.Commands.CreateAccess;
using BackOffice.Application.Features.Admin.Commands.CreateAdmin;
using BackOffice.Application.Features.Admin.Commands.CreateProfil;
using BackOffice.Application.Features.Admin.Commands.CreateRedevable;
using BackOffice.Application.Features.Admin.Queries.GetUserTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers;

/// <summary>
/// Contrôleur dédié à la gestion de l'administration du système (Accès, Profils, Utilisateurs).
/// Accessible uniquement aux administrateurs (gestion des droits à affiner plus tard).
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize] 
public class AdminController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crée un nouvel accès (permission) dans le système.
    /// </summary>
    /// <param name="command">Les détails de l'accès à créer.</param>
    /// <returns>Un succès si l'accès est créé.</returns>
    [HttpPost("acces")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAccess([FromBody] CreateAccessCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Crée un nouveau profil (rôle) et lui associe une liste de droits d'accès.
    /// </summary>
    /// <param name="command">Les détails du profil et la liste des accès associés.</param>
    /// <returns>Un succès si le profil est créé.</returns>
    [HttpPost("profils")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProfil([FromBody] CreateProfilCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Crée un nouvel utilisateur administrateur (agent) dans le Back Office.
    /// </summary>
    /// <param name="command">Les informations du nouvel administrateur.</param>
    /// <returns>Un succès si l'utilisateur est créé.</returns>
    [HttpPost("utilisateurs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }


    /// <summary>
    /// Attribue un profil spécifique à un utilisateur administrateur.
    /// </summary>
    /// <param name="command">Contient l'ID de l'utilisateur et l'ID du profil.</param>
    /// <returns>Un succès si l'attribution est effectuée.</returns>
    [HttpPatch("utilisateurs/attribution-profil")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignProfilToAdmin([FromBody] AssignProfilToAdminCommand command)
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

    /// <summary>
    /// Modifie le mot de passe d'un utilisateur administrateur.
    /// </summary>
    /// <param name="command">Contient l'ID de l'utilisateur et le nouveau mot de passe.</param>
    /// <returns>Un succès si le changement est effectué.</returns>
    [HttpPatch("utilisateurs/changement-mot-de-passe")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeAdminPassword([FromBody] ChangeAdminPasswordCommand command)
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

    /// <summary>
    /// Récupère la liste des types d'utilisateurs (ex: Administrateur, Utilisateur Standard).
    /// </summary>
    [HttpGet("types-utilisateur")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AdminUserTypeDto>))]
    public async Task<IActionResult> GetUserTypes()
    {
        var result = await _mediator.Send(new GetAdminUserTypesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Pour la création du compte client(redvable)
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("redevables")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateRedevable([FromBody] CreateRedevableCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPatch("utilisateurs/ldap/profil")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignProfilToLdapUser([FromBody] AssignProfilToLdapUserCommand command)
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
}