using FrontOffice.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Clients.Commands.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly INotificationService _notificationService; 

    public DeleteAccountCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPasswordHasher passwordHasher,
        INotificationService notificationService) 
    {
        _context = context;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException();

        var client = await _context.Clients.FindAsync(userId.Value);
        if (client == null) throw new UnauthorizedAccessException();

        if (!_passwordHasher.Verify(request.Password, client.MotPasse!))
        {
            throw new InvalidOperationException("Le mot de passe est incorrect.");
        }

        client.Desactive = 1;
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.SendToGroupAsync(
            groupName: "ADMIN",
            title: "Compte Client Désactivé",
            message: $"Le client '{client.RaisonSociale}' a désactivé son compte.",
            type: "Warning" 
        );

        return true;
    }
}