using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegisterClientCommandHandler> _logger;
    private readonly ICaptchaValidator _captchaValidator;
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initialise une nouvelle instance du handler avec toutes les dépendances nécessaires.
    /// </summary>
    public RegisterClientCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<RegisterClientCommandHandler> logger,
        ICaptchaValidator captchaValidator,
        INotificationService notificationService)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _captchaValidator = captchaValidator;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Point d'entrée principal pour le traitement de la commande.
    /// </summary>
    public async Task<AuthenticationResult> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        // ... (Logique Captcha si activée)

        var existingClient = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        if (existingClient != null)
        {
            if (existingClient.NiveauValidation > 0 || existingClient.IsVerified)
            {
                throw new InvalidOperationException("Un compte client avec cet e-mail existe déjà et est en cours de validation ou actif.");
            }
            return await ResendVerificationAsync(existingClient, cancellationToken);
        }

        return await CreateNewClientAsync(request, cancellationToken);
    }

    /// <summary>
    /// Crée un nouveau client dans la base de données avec un statut initial "Non Vérifié".
    /// </summary>
    private async Task<AuthenticationResult> CreateNewClientAsync(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.Hash(request.Password);
        var verificationCode = GenerateVerificationCode();
        long verificationTokenExpiry = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeMilliseconds();

        var newClient = new Client
        {
            Id = Guid.NewGuid(),
            Code = $"CLT-{DateTime.UtcNow.Ticks}",
            MotPasse = hashedPassword,
            RaisonSociale = request.RaisonSociale,
            Email = request.Email,
            ContactNom = request.ContactNom,
            ContactTelephone = request.ContactTelephone,
            ContactFonction = request.ContactFonction,
            TypeClient = request.TypeClient,
            RegistreCommerce = request.RegistreCommerce,
            Adresse = request.Adresse,
            Bp = request.Bp,
            Ville = request.Ville,
            Pays = request.Pays,
            Desactive = 0,
            ChangementMotPasse = 1,
            IsVerified = false,
            NiveauValidation = 0,
            VerificationCode = verificationCode,
            VerificationTokenExpiry = verificationTokenExpiry,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_REGISTER"
        };

        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync(cancellationToken);

        var verificationToken = _jwtTokenGenerator.GenerateToken(newClient.Id, newClient.Email);

        //await SendVerificationEmailAsync(newClient.Email, verificationCode);
        //await SendNotificationToArpceAsync(newClient);

        await _notificationService.SendToGroupAsync(
            groupName: "ADMIN",
            title: "Nouvelle Inscription Redevable",
            message: $"Un nouveau redevable '{newClient.RaisonSociale}' s'est inscrit.",
            type: "E", 
            targetUrl: $"/redevables/{newClient.Id}" 
        );

        return new AuthenticationResult
        {
            Message = "Inscription enregistrée avec succès. Veuillez vérifier votre e-mail pour récupérer votre code de confirmation.",
            Token = verificationToken
        };
    }

    /// <summary>
    /// Gère le cas où un utilisateur tente de se réinscrire alors que son compte n'est pas encore validé par OTP.
    /// </summary>
    private async Task<AuthenticationResult> ResendVerificationAsync(Client client, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Demande de renvoi de code OTP pour le client {Email}", client.Email);

        client.VerificationCode = GenerateVerificationCode();
        client.VerificationTokenExpiry = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeMilliseconds();

        await _context.SaveChangesAsync(cancellationToken);

        var verificationToken = _jwtTokenGenerator.GenerateToken(client.Id, client.Email!);
        //await SendVerificationEmailAsync(client.Email!, client.VerificationCode);

        return new AuthenticationResult
        {
            Message = "Un nouveau code de confirmation a été envoyé à votre adresse e-mail.",
            Token = verificationToken
        };
    }

    /// <summary>
    /// Génère un code numérique aléatoire à 6 chiffres.
    /// </summary>
    private string GenerateVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString("D6");
    }

    /// <summary>
    /// Envoie l'e-mail OTP au client via le service d'email.
    /// </summary>
    private async Task SendVerificationEmailAsync(string email, string code)
    {
        var subject = "Code de vérification - ARPCE Homologation";
        var body = $@"<p>Votre code de vérification est : <strong>{code}</strong></p>";
        await _emailService.SendEmailAsync(email, subject, body);
    }

    /// <summary>
    /// Envoie une notification par e-mail à l'administrateur ARPCE pour l'informer d'une nouvelle inscription.
    /// </summary>
    private async Task SendNotificationToArpceAsync(Client client)
    {
        var arpceEmail = _configuration["EmailSettings:NotificationEmail"];
        if (string.IsNullOrEmpty(arpceEmail))
        {
            _logger.LogWarning("Email de notification ARPCE non configuré. La notification n'a pas été envoyée.");
            return;
        }

        var subject = $"[NOUVELLE INSCRIPTION] - {client.RaisonSociale}";
        var body = $@"<h2>Nouvelle demande d'inscription</h2><p>Le compte pour {client.RaisonSociale} ({client.Email}) a été créé et attend la validation OTP.</p>";
        try
        {
            await _emailService.SendEmailAsync(arpceEmail, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de la notification d'inscription à l'admin.");
        }
    }
}