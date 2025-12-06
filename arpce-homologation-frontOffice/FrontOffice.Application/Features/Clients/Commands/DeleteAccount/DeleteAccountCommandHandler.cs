using FrontOffice.Application.Common.Interfaces;
using MediatR;

namespace FrontOffice.Application.Features.Clients.Commands.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public DeleteAccountCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IPasswordHasher passwordHasher)
    {
        context = _context;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException();

        var client = await _context.Clients.FindAsync(userId.Value);
        if (client == null) throw new UnauthorizedAccessException();

        // Vérification du mot de passe
        if (!_passwordHasher.Verify(request.Password, client.MotPasse!))
        {
            throw new InvalidOperationException("Le mot de passe est incorrect.");
        }

        // Suppression logique (désactivation)
        client.Desactive = 1;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}