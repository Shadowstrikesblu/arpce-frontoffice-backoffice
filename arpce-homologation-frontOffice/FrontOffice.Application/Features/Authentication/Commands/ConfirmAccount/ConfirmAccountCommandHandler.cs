using FrontOffice.Application.Common.Interfaces;
using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ConfirmAccount;

public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator; 

    public ConfirmAccountCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IJwtTokenGenerator jwtTokenGenerator) 
    {
        _context = context;
        _currentUserService = currentUserService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticationResult> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Le token de vérification est invalide, manquant ou a expiré.");
        }

        var client = await _context.Clients.FindAsync(new object[] { userId.Value }, cancellationToken);

        if (client == null)
        {
            throw new InvalidOperationException("Compte utilisateur introuvable.");
        }

        if (client.IsVerified || client.NiveauValidation >= 2)
        {
            throw new InvalidOperationException("Ce compte a déjà été vérifié.");
        }

        long nowAsUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (client.VerificationTokenExpiry < nowAsUnixTimestamp)
        {
            throw new InvalidOperationException("Le code de vérification a expiré.");
        }

        if (string.IsNullOrWhiteSpace(request.Code) || client.VerificationCode?.Trim() != request.Code.Trim())
        {
            throw new InvalidOperationException("Le code de vérification est incorrect.");
        }

        client.NiveauValidation = 2;
        client.IsVerified = true;
        client.VerificationCode = null;
        client.VerificationTokenExpiry = null;

        await _context.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(client.Id, client.Email!);

        return new AuthenticationResult
        {
            Message = "Votre e-mail a été vérifié avec succès. Vous êtes maintenant connecté.",
            Token = token 
        };
    }
}