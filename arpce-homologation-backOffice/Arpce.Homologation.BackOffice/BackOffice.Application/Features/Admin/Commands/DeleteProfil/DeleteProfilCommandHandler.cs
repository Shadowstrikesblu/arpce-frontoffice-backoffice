using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.DeleteProfil;

public class DeleteProfilCommandHandler : IRequestHandler<DeleteProfilCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProfilCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProfilCommand request, CancellationToken cancellationToken)
    {
        var profil = await _context.AdminProfils.FindAsync(new object[] { request.Id }, cancellationToken);

        if (profil == null)
        {
            throw new Exception($"Le profil avec l'ID '{request.Id}' est introuvable.");
        }

        // Suppression physique. Les relations (AdminProfilsAcces) seront supprimées en cascade.
        // Les AdminUtilisateurs verront leur IdProfil passer à NULL.
        _context.AdminProfils.Remove(profil);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}