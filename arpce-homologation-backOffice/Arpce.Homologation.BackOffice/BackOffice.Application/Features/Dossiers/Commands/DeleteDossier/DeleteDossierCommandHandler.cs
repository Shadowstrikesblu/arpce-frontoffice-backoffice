using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Dossiers.Commands.DeleteDossier;

public class DeleteDossierCommandHandler : IRequestHandler<DeleteDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteDossierCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteDossierCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers.FindAsync(new object[] { request.Id }, cancellationToken);

        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.Id}' est introuvable.");
        }

        // Suppression physique du dossier.
        // EF Core gérera la suppression en cascade des enfants (Demandes, etc.) 
        // grâce à .OnDelete(DeleteBehavior.Cascade) configuré dans DossierConfiguration.
        _context.Dossiers.Remove(dossier);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}