using FrontOffice.Application.Common.Interfaces;
using MediatR;
using System.Security.Cryptography;

namespace FrontOffice.Application.Features.Authentication.Commands.ResendOtp;

public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public ResendOtpCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IEmailService emailService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<bool> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Token invalide.");

        var client = await _context.Clients.FindAsync(new object[] { userId.Value }, cancellationToken);
        if (client == null) throw new InvalidOperationException("Utilisateur introuvable.");

        // Générer un nouveau code
        var newCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString("D6");
        client.VerificationCode = newCode;
        client.VerificationTokenExpiry = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeMilliseconds();

        await _context.SaveChangesAsync(cancellationToken);

        // Envoyer l'e-mail
        var subject = "Nouveau code de vérification ARPCE";
        var body = $"<h1>Votre nouveau code : {newCode}</h1>";
        await _emailService.SendEmailAsync(client.Email!, subject, body);

        return true;
    }
}