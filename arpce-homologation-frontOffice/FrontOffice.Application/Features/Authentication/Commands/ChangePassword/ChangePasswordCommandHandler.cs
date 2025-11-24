using FrontOffice.Application.Common.Interfaces;
using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IPasswordHasher passwordHasher)
    {
        _context = context;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Non authentifié.");

        var client = await _context.Clients.FindAsync(new object[] { userId.Value }, cancellationToken);
        if (client == null) throw new UnauthorizedAccessException("Utilisateur introuvable.");

        // Vérification de l'ancien mot de passe
        if (!_passwordHasher.Verify(request.AncienMotDePasse, client.MotPasse!))
        {
            throw new InvalidOperationException("L'ancien mot de passe est incorrect.");
        }

        // Mise à jour avec le nouveau
        client.MotPasse = _passwordHasher.Hash(request.NouveauMotDePasse);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}