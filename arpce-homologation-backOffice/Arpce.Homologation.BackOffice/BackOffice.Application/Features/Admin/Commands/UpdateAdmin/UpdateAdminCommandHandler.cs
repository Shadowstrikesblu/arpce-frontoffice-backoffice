using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.UpdateAdmin;

public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateAdminCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _context.AdminUtilisateurs.FindAsync(new object[] { request.Id }, cancellationToken);
        if (admin == null) throw new Exception("Administrateur introuvable.");

        if (request.Compte != null) admin.Compte = request.Compte;
        if (request.Nom != null) admin.Nom = request.Nom;
        if (request.Prenoms != null) admin.Prenoms = request.Prenoms;
        if (request.Remarques != null) admin.Remarques = request.Remarques;
        if (request.IdUtilisateurType.HasValue) admin.IdUtilisateurType = request.IdUtilisateurType.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}