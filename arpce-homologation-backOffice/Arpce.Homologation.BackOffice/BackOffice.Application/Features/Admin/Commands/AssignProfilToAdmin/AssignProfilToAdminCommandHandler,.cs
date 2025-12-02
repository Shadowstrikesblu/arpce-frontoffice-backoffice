using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Admin.Commands.AssignProfilToAdmin;

/// <summary>
/// Gère la logique d'attribution d'un profil à un administrateur.
/// </summary>
public class AssignProfilToAdminCommandHandler : IRequestHandler<AssignProfilToAdminCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public AssignProfilToAdminCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(AssignProfilToAdminCommand request, CancellationToken cancellationToken)
    {
        // Récupére l'utilisateur administrateur
        var admin = await _context.AdminUtilisateurs.FindAsync(new object[] { request.UtilisateurId }, cancellationToken);

        if (admin == null)
        {
            throw new Exception($"L'utilisateur avec l'ID '{request.UtilisateurId}' est introuvable.");
        }

        // Vérifie que le profil existe
        var profilExists = await _context.AdminProfils.AnyAsync(p => p.Id == request.IdProfil, cancellationToken);

        if (!profilExists)
        {
            throw new Exception($"Le profil avec l'ID '{request.IdProfil}' est introuvable.");
        }

        // Assigne le profil
        admin.IdProfil = request.IdProfil;

        //  Sauvegarde les changements
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
     page: "Gestion des Utilisateurs",
     libelle: $"Attribution du profil ID '{request.IdProfil}' à l'utilisateur ID '{request.UtilisateurId}'.",
     eventTypeCode: "ATTRIBUTION");

        return true;
    }
}