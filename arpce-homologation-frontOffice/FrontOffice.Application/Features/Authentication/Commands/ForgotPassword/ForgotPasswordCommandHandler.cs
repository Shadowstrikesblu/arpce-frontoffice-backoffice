using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    /// <summary>
    /// Constructeur pour l'injection des dépendances.
    /// </summary>
    public ForgotPasswordCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _emailService = emailService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticationResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        // Variable pour stocker le token temporaire
        string temporaryToken = "";

        // Sécurité : On ne dit jamais si l'email existe ou non.
        if (client != null && client.Desactive != 1)
        {
            // Génére un OTP et une date d'expiration
            var code = new Random().Next(100000, 999999).ToString("D6");
            client.VerificationCode = code;
            client.VerificationTokenExpiry = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeMilliseconds();

            await _context.SaveChangesAsync(cancellationToken);

            // Envoye l'email
            var subject = "Réinitialisation de votre mot de passe - ARPCE";
            var body = $"<p>Bonjour,</p><p>Votre code de réinitialisation est : <strong>{code}</strong></p><p>Ce code est valable 15 minutes.</p>";
            await _emailService.SendEmailAsync(client.Email, subject, body);

            temporaryToken = _jwtTokenGenerator.GenerateToken(client.Id, client.Email);
        }

        // On retourne un message de succès générique et le token (qui sera vide si le client n'existe pas).
        return new AuthenticationResult
        {
            Message = "Si un compte est associé à cet e-mail, un code de réinitialisation a été envoyé.",
            Token = temporaryToken
        };
    }
}