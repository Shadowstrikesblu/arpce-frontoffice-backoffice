// Fichier : BackOffice.Api/Controllers/AdminController.cs

// --- Usings pour les Commandes et Requêtes MediatR ---
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
using BackOffice.Application.Features.Admin.Commands.UpdateAccess;
using BackOffice.Application.Features.Admin.Commands.UpdateAdmin;
using BackOffice.Application.Features.Admin.Commands.UpdateProfil;
using BackOffice.Application.Features.Admin.Commands.UpdateRedevable;
using BackOffice.Application.Features.Admin.Commands.ValidateRedevable;
using BackOffice.Application.Features.Admin.Queries.GetAccessList;
using BackOffice.Application.Features.Admin.Queries.GetAdminJournalList;
using BackOffice.Application.Features.Admin.Queries.GetAdminUserDetail;
using BackOffice.Application.Features.Admin.Queries.GetAdminUsersList;
using BackOffice.Application.Features.Admin.Queries.GetProfilsList;
using BackOffice.Application.Features.Admin.Queries.GetRedevableDetail;
using BackOffice.Application.Features.Admin.Queries.GetRedevablesAValider;
using BackOffice.Application.Features.Admin.Queries.GetRedevablesList;
using BackOffice.Application.Features.Admin.Queries.GetUserTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackOffice.Api.Controllers;

/// <summary>
/// Contrôleur centralisé pour toutes les opérations d'administration du système.
/// Gère les utilisateurs (admins, redevables), les profils et les droits d'accès.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize] // Sécurisé par défaut, nécessite un token d'agent valide
public class AdminController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminController(ISender mediator)
    {
        _mediator = mediator;
    }

    // --- GESTION DES UTILISATEURS ADMINS ---

    /// <summary>
    /// Crée un nouvel utilisateur administrateur (agent).
    /// </summary>
    [HttpPost("utilisateurs")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Récupère la liste paginée des utilisateurs administrateurs.
    /// </summary>
    [HttpGet("utilisateurs")]
    public async Task<IActionResult> GetAdminUsers([FromQuery] GetAdminUsersListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Récupère les détails complets d'un utilisateur administrateur.
    /// </summary>
    [HttpGet("utilisateurs/{utilisateurId:guid}")]
    public async Task<IActionResult> GetAdminUserDetail(Guid utilisateurId)
    {
        var result = await _mediator.Send(new GetAdminUserDetailQuery(utilisateurId));
        return Ok(result);
    }

    /// <summary>
    /// Modifie les informations d'un utilisateur administrateur.
    /// </summary>
    [HttpPatch("utilisateurs/{id:guid}")]
    public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] UpdateAdminCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Supprime un utilisateur administrateur.
    /// </summary>
    [HttpDelete("utilisateurs/{id:guid}")]
    public async Task<IActionResult> DeleteAdmin(Guid id)
    {
        var result = await _mediator.Send(new DeleteAdminCommand(id));
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Modifie le mot de passe d'un utilisateur administrateur.
    /// </summary>
    [HttpPatch("utilisateurs/changement-mot-de-passe")]
    public async Task<IActionResult> ChangeAdminPassword([FromBody] ChangeAdminPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Récupère la liste des types d'utilisateurs (ex: Administrateur).
    /// </summary>
    [HttpGet("types-utilisateur")]
    public async Task<IActionResult> GetUserTypes()
    {
        var result = await _mediator.Send(new GetAdminUserTypesQuery());
        return Ok(result);
    }

    // --- GESTION DES PROFILS ET ACCÈS ---

    /// <summary>
    /// Crée un nouveau profil (rôle) avec ses droits d'accès.
    /// </summary>
    [HttpPost("profils")]
    public async Task<IActionResult> CreateProfil([FromBody] CreateProfilCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Récupère la liste paginée des profils avec leurs détails.
    /// </summary>
    [HttpGet("profils")]
    public async Task<IActionResult> GetProfils([FromQuery] GetProfilsListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Supprime un profil.
    /// </summary>
    [HttpDelete("profils/{id:guid}")]
    public async Task<IActionResult> DeleteProfil(Guid id)
    {
        var result = await _mediator.Send(new DeleteProfilCommand(id));
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Modifie un profil existant.
    /// </summary>
    [HttpPatch("profils/{id:guid}")] // Ou [HttpPut] selon votre convention
    public async Task<IActionResult> UpdateProfil(Guid id, [FromBody] UpdateProfilCommand command)
    {
        if (id != command.Id)
        {
            command.Id = id;
        }
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Attribue un profil à un utilisateur administrateur.
    /// </summary>
    [HttpPatch("utilisateurs/attribution-profil")]
    public async Task<IActionResult> AssignProfilToAdmin([FromBody] AssignProfilToAdminCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Attribue un profil à un utilisateur LDAP.
    /// </summary>
    [HttpPatch("utilisateurs/ldap/profil")]
    public async Task<IActionResult> AssignProfilToLdapUser([FromBody] AssignProfilToLdapUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Crée un nouvel accès (permission) dans le système.
    /// </summary>
    [HttpPost("acces")]
    public async Task<IActionResult> CreateAccess([FromBody] CreateAccessCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Récupère la liste de tous les accès disponibles.
    /// </summary>
    [HttpGet("acces")]
    public async Task<IActionResult> GetAccessList()
    {
        var result = await _mediator.Send(new GetAccessListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Supprime un accès.
    /// </summary>
    [HttpDelete("acces/{id:guid}")]
    public async Task<IActionResult> DeleteAcces(Guid id)
    {
        var result = await _mediator.Send(new DeleteAccesCommand(id));
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Modifie un accès existant.
    /// </summary>
    [HttpPatch("acces/{id:guid}")] 
    public async Task<IActionResult> UpdateAccess(Guid id, [FromBody] UpdateAccessCommand command)
    {
        if (id != command.Id)
        {
            command.Id = id;
        }
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    // --- GESTION DES REDEVABLES ---

    /// <summary>
    /// Crée un compte redevable (client) depuis le Back Office.
    /// </summary>
    [HttpPost("redevables")]
    public async Task<IActionResult> CreateRedevable([FromBody] CreateRedevableCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Récupère la liste paginée des redevables.
    /// </summary>
    [HttpGet("redevables")]
    public async Task<IActionResult> GetRedevables([FromQuery] GetRedevablesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Récupère les détails complets d'un redevable.
    /// </summary>
    [HttpGet("redevables/{utilisateurId:guid}")]
    public async Task<IActionResult> GetRedevableDetail(Guid utilisateurId)
    {
        var result = await _mediator.Send(new GetRedevableDetailQuery(utilisateurId));
        return Ok(result);
    }

    /// <summary>
    /// Modifie les informations d'un redevable.
    /// </summary>
    [HttpPatch("redevables/{id:guid}")]
    public async Task<IActionResult> UpdateRedevable(Guid id, [FromBody] UpdateRedevableCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Supprime (désactive) un compte redevable.
    /// </summary>
    [HttpDelete("redevables/{id:guid}")]
    public async Task<IActionResult> DeleteRedevable(Guid id)
    {
        var result = await _mediator.Send(new DeleteRedevableCommand(id));
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Valide un compte redevable qui a déjà vérifié son email (Niveau 1 -> 2).
    /// </summary>
    [HttpPost("redevables/{id:guid}/valider")]
    public async Task<IActionResult> ValidateRedevable(Guid id)
    {
        var result = await _mediator.Send(new ValidateRedevableCommand { RedevableId = id });
        return Ok(new { ok = result });
    }

    /// <summary>
    /// Récupère la liste des redevables en attente de validation administrative (Niveau 1).
    /// </summary>
    [HttpGet("redevables/a-valider")]
    public async Task<IActionResult> GetRedevablesAValider([FromQuery] GetRedevablesAValiderQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Pour l'audit
    /// </summary>
    /// <returns></returns>
    [HttpGet("journal")]
    public async Task<IActionResult> GetAdminJournalList()
    {
        var result = await _mediator.Send(new GetAdminJournalListQuery());
        return Ok(result);
    }
}