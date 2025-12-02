using BackOffice.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Commands.DeleteAdmin;

public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public DeleteAdminCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _context.AdminUtilisateurs.FindAsync(new object[] { request.Id }, cancellationToken);
        if (admin == null) throw new Exception("Administrateur introuvable.");

        // Suppression physique pour un admin (sauf si c'est l'admin par défaut, à gérer ?)
        _context.AdminUtilisateurs.Remove(admin);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion des Utilisateurs",
    libelle: $"Suppression de l'utilisateur admin '{admin.Compte}' (ID: {admin.Id}).",
    eventTypeCode: "SUPPRESSION");

        return true;
    }
}