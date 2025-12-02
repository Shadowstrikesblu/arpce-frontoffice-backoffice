using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Admin.Commands.AssignProfilToLdapUser;

public class AssignProfilToLdapUserCommandHandler : IRequestHandler<AssignProfilToLdapUserCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public AssignProfilToLdapUserCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(AssignProfilToLdapUserCommand request, CancellationToken cancellationToken)
    {
        // Vérifie que le profil existe
        var profilExists = await _context.AdminProfils.AnyAsync(p => p.Id == request.IdProfil, cancellationToken);
        if (!profilExists)
        {
            throw new Exception($"Le profil avec l'ID '{request.IdProfil}' est introuvable.");
        }

        // Vérifie si une liaison existe déjà pour cet utilisateur
        var existingLink = await _context.AdminProfilsUtilisateursLDAP
            .FirstOrDefaultAsync(u => u.Utilisateur == request.Utilisateur, cancellationToken);

        if (existingLink != null)
        {
            // Mise à jour du profil existant
            existingLink.IdProfil = request.IdProfil;
        }
        else
        {
            // Création d'une nouvelle liaison
            var newLink = new AdminProfilsUtilisateursLDAP
            {
                Utilisateur = request.Utilisateur,
                IdProfil = request.IdProfil
            };
            _context.AdminProfilsUtilisateursLDAP.Add(newLink);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion Utilisateurs LDAP",
    libelle: $"Attribution du profil ID '{request.IdProfil}' à l'utilisateur LDAP '{request.Utilisateur}'.",
    eventTypeCode: "ATTRIBUTION");

        return true;
    }
}