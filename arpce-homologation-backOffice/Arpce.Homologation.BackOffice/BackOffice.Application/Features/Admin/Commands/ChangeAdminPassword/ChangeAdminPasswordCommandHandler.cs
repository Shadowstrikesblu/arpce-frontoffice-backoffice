using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.ChangeAdminPassword;

public class ChangeAdminPasswordCommandHandler : IRequestHandler<ChangeAdminPasswordCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ChangeAdminPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(ChangeAdminPasswordCommand request, CancellationToken cancellationToken)
    {
        // Récupére l'utilisateur
        var admin = await _context.AdminUtilisateurs.FindAsync(new object[] { request.UtilisateurId }, cancellationToken);

        if (admin == null)
        {
            throw new Exception($"L'utilisateur avec l'ID '{request.UtilisateurId}' est introuvable.");
        }

        // Hache le nouveau mot de passe
        var hashedPassword = _passwordHasher.Hash(request.MotDePasse);

        // Met à jour le mot de passe
        admin.MotPasse = hashedPassword;

        // Force le changement de mot de passe à la prochaine connexion
        admin.ChangementMotPasse = true; 

        // Sauvegarde
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}