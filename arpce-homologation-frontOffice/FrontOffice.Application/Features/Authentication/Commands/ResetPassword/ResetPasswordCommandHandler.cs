using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    public ResetPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        if (client == null || client.VerificationCode != request.OtpCode || client.VerificationTokenExpiry < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Code invalide ou expiré.");
        }

        // Mettre à jour le mot de passe
        client.MotPasse = _passwordHasher.Hash(request.NouveauMotDePasse);

        // Nettoyer les champs de vérification
        client.VerificationCode = null;
        client.VerificationTokenExpiry = null;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}