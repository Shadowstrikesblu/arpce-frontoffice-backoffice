using BackOffice.Application.Features.Admin.Commands.AssignProfilToAdmin;
using BackOffice.Application.Features.Admin.Commands.AssignProfilToLdapUser;
using BackOffice.Application.Features.Admin.Commands.ChangeAdminPassword;
using BackOffice.Application.Features.Admin.Commands.CreateAccess;
using BackOffice.Application.Features.Admin.Commands.CreateAdmin;
using BackOffice.Application.Features.Admin.Commands.CreateProfil;
using BackOffice.Application.Features.Admin.Commands.CreateRedevable;
using BackOffice.Application.Features.Admin.Commands.DeleteAcces;
using BackOffice.Application.Features.Admin.Commands.DeleteAdmin;
using BackOffice.Application.Features.Admin.Commands.DeleteProfil;
using BackOffice.Application.Features.Admin.Commands.DeleteRedevable;
using BackOffice.Application.Features.Admin.Commands.UpdateAdmin;
using BackOffice.Application.Features.Admin.Commands.UpdateRedevable;
using BackOffice.Application.Features.Admin.Commands.ValidateRedevable;
using BackOffice.Application.Features.Admin.Queries.GetAccessList;
using BackOffice.Application.Features.Admin.Queries.GetAdminJournalList;
using BackOffice.Application.Features.Admin.Queries.GetAdminUserDetail;
using BackOffice.Application.Features.Admin.Queries.GetAdminUsersList;
using BackOffice.Application.Features.Admin.Queries.GetRedevableDetail;
using BackOffice.Application.Features.Admin.Queries.GetRedevablesList;
using BackOffice.Application.Features.Admin.Queries.GetUserTypes;
using BackOffice.Application.Features.Authentication.Queries.CheckToken;
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

    /// <summary>
    /// Pour la création du compte client(redvable)
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create-redevables")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateRedevable([FromBody] CreateRedevableCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Récupère la liste des redevables avec filtres et pagination.
    /// </summary>
    [HttpGet("redevables")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RedevableListVm))]
    public async Task<IActionResult> GetRedevables([FromQuery] GetRedevablesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Récupère les détails complets d'un redevable.
    /// </summary>
    [HttpGet("redevables/{utilisateurId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RedevableDetailDto))]
    public async Task<IActionResult> GetRedevableDetail(Guid utilisateurId)
    {
        try
        {
            var result = await _mediator.Send(new GetRedevableDetailQuery(utilisateurId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { title = "Non trouvé", detail = ex.Message });
        }
    }

    /// <summary>
    /// Récupère la liste des utilisateurs
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("utilisateurs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminUserListVm))]
    public async Task<IActionResult> GetAdminUsers([FromQuery] GetAdminUsersListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Récupère la liste des détailles 
    /// </summary>
    /// <param name="utilisateurId"></param>
    /// <returns></returns>
    [HttpGet("utilisateurs/{utilisateurId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminUserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAdminUserDetail(Guid utilisateurId)
    {
        try
        {
            var result = await _mediator.Send(new GetAdminUserDetailQuery(utilisateurId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { title = "Non trouvé", detail = ex.Message });
        }
    }

    /// <summary>
    /// Récupère la liste des accès
    /// </summary>
    /// <returns></returns>
    [HttpGet("acces")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AdminAccessDto>))]
    public async Task<IActionResult> GetAccessList()
    {
        var result = await _mediator.Send(new GetAccessListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Pour valider le compte du redevable
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("redevables/{id:guid}/valider")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> ValidateRedevable(Guid id)
    {
        await _mediator.Send(new ValidateRedevableCommand { RedevableId = id });
        return Ok(new { ok = true });
    }

    /// <summary>
    /// Pour la récupération de la liste de journal
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("journal")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JournalListVm))]
    public async Task<IActionResult> GetJournalList([FromQuery] GetAdminJournalListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Mis à jour du redvable
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPatch("redevables/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateRedevable(Guid id, [FromBody] UpdateRedevableCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Suppression du redvable
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("redevables/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> DeleteRedevable(Guid id)
    {
        var result = await _mediator.Send(new DeleteRedevableCommand(id));
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Mis à jour de l'utilisateur ARPCE
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPatch("utilisateurs/{id:guid}")]
    public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] UpdateAdminCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Suppression de l'utilisateur ARPCE
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("utilisateurs/{id:guid}")]
    public async Task<IActionResult> DeleteAdmin(Guid id)
    {
        var result = await _mediator.Send(new DeleteAdminCommand(id));
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Supression du profil
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("profils/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfil(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteProfilCommand(id));
            return Ok(new { ok = result });
        }
        catch (Exception ex) when (ex.Message.Contains("introuvable"))
        {
            return NotFound(new { title = "Non trouvé", detail = ex.Message });
        }
    }

    /// <summary>
    /// Suppression des accès utilisateurs
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("acces/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> DeleteAcces(Guid id)
    {
        var result = await _mediator.Send(new DeleteAccesCommand(id));
        return Ok(new { ok = result });
    }
}