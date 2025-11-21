using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;

namespace FrontOffice.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Gère la logique de la commande d'inscription d'un nouveau client.
/// </summary>
public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterClientCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<AuthenticationResult> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        var existingClient = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        if (existingClient != null)
        {
            if (existingClient.IsVerified)
            {
                throw new InvalidOperationException("Un compte client avec cet e-mail existe déjà et est actif.");
            }
            return await ResendVerificationAsync(existingClient, cancellationToken);
        }

        return await CreateNewClientAsync(request, cancellationToken);
    }

    private async Task<AuthenticationResult> CreateNewClientAsync(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.Hash(request.Password);
        var verificationCode = GenerateVerificationCode();
        var verificationTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        var newClient = new Client
        {
            RaisonSociale = request.RaisonSociale,
            Email = request.Email,
            MotPasse = hashedPassword,
            ContactNom = request.ContactNom,
            ContactTelephone = request.ContactTelephone,

            Code = $"CLT-{DateTime.UtcNow.Ticks}",
            Desactive = 0,
            ChangementMotPasse = 1,
            IsVerified = false,
            VerificationCode = verificationCode,
            VerificationTokenExpiry = verificationTokenExpiry
        };
        // -----------------------------------------------------------------------------------

        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync(cancellationToken);

        var verificationToken = _jwtTokenGenerator.GenerateToken(newClient.Id, newClient.Email);

        await SendVerificationEmailAsync(newClient.Email, verificationCode);

        return new AuthenticationResult
        {
            Message = "Inscription réussie. Veuillez vérifier votre e-mail pour le code de confirmation.",
            Token = verificationToken
        };
    }

    private async Task<AuthenticationResult> ResendVerificationAsync(Client client, CancellationToken cancellationToken)
    {
        client.VerificationCode = GenerateVerificationCode();
        client.VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        await _context.SaveChangesAsync(cancellationToken);

        var verificationToken = _jwtTokenGenerator.GenerateToken(client.Id, client.Email!);

        await SendVerificationEmailAsync(client.Email!, client.VerificationCode);

        return new AuthenticationResult
        {
            Message = "Un nouveau code de confirmation a été envoyé à votre adresse e-mail.",
            Token = verificationToken
        };
    }

    private string GenerateVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString("D6");
    }

    private async Task SendVerificationEmailAsync(string email, string code)
    {
        var subject = "Votre code de vérification pour vous connecter sur l'espace client de la plateforme ARPCE Homologation";
        var body = $@"
            <h1>Bienvenue sur la plateforme d'homologation de l'ARPCE</h1>
            <p>Merci de vous être inscrit. Veuillez utiliser le code ci-dessous pour activer votre compte :</p>
            <h2><strong>{code}</strong></h2>
            <p>Ce code expirera dans 30 minutes.</p>
            <p>Si vous n'êtes pas à l'origine de cette inscription, veuillez ignorer cet e-mail.</p>
            <p>Cordialement,<br/>L'équipe ARPCE</p>";

        await _emailService.SendEmailAsync(email, subject, body);
    }
}