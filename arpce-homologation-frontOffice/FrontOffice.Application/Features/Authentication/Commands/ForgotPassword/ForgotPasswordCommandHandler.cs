using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    // ... constructeur ...

    public async Task<AuthenticationResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        // Sécurité : On ne dit jamais si l'email existe ou non.
        if (client != null)
        {
            // Générer un OTP et une date d'expiration
            var code = new Random().Next(100000, 999999).ToString();
            client.VerificationCode = code;
            client.VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync(cancellationToken);

            // Envoyer l'email
            await _emailService.SendEmailAsync(client.Email, "Réinitialisation mot de passe", $"Votre code est : {code}");
        }

        // On retourne un succès (vide) dans tous les cas pour ne pas donner d'infos
        return new AuthenticationResult
        {
            Message = "Si un compte est associé à cet e-mail, un code de réinitialisation a été envoyé.",
            Token = "" // On pourrait générer un token temporaire si le front le demande
        };
    }
}