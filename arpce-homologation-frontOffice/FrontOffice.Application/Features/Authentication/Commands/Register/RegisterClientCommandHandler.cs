using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace FrontOffice.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Gère la logique de la commande d'inscription d'un nouveau client.
/// Workflow : Validation Captcha -> Vérification doublon -> Création (Niveau 0) -> Envoi OTP -> Notification Admin ARPCE.
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
        ICaptchaValidator captchaValidator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _captchaValidator = captchaValidator;
    }

    /// <summary>
    /// Point d'entrée principal pour le traitement de la commande.
    /// </summary>
    public async Task<AuthenticationResult> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        // Validation de Sécurité (Captcha)
        // Si un token Captcha est fourni par le frontend, nous le validons auprès de Google.
        // Si la validation échoue, nous rejetons la demande immédiatement.
        if (!string.IsNullOrWhiteSpace(request.CaptchaToken))
        {
            bool isCaptchaValid = await _captchaValidator.ValidateAsync(request.CaptchaToken);
            if (!isCaptchaValid)
            {
                _logger.LogWarning("Tentative d'inscription avec un Captcha invalide. Email: {Email}", request.Email);
                throw new InvalidOperationException("La validation de sécurité (Captcha) a échoué. Veuillez réessayer.");
            }
        }
        else
        {
            // Politique de sécurité : Le Captcha est-il obligatoire ?
            // Pour l'instant, on log un avertissement si manquant, mais on peut choisir de bloquer (décommenter la ligne suivante).
            _logger.LogWarning("Inscription demandée sans token Captcha. Email: {Email}", request.Email);
            //throw new InvalidOperationException("Le token Captcha est requis pour l'inscription.");
        }

        // Vérification de l'existence du client (Doublon d'e-mail)
        var existingClient = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        if (existingClient != null)
        {
            // Si le compte est déjà au niveau 1 (OTP Validé) ou 2 (Validé ARPCE), on empêche la réinscription.
            if (existingClient.NiveauValidation > 0 || existingClient.IsVerified)
            {
                throw new InvalidOperationException("Un compte client avec cet e-mail existe déjà et est en cours de validation ou actif.");
            }

            return await ResendVerificationAsync(existingClient, cancellationToken);
        }

        // Création d'un nouveau client (Cas nominal)
        return await CreateNewClientAsync(request, cancellationToken);
    }

    /// <summary>
    /// Crée un nouveau client dans la base de données avec un statut initial "Non Vérifié".
    /// </summary>
    private async Task<AuthenticationResult> CreateNewClientAsync(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        // Hachage du mot de passe pour le stockage sécurisé
        var hashedPassword = _passwordHasher.Hash(request.Password);

        // Génération du code OTP (6 chiffres) et de sa date d'expiration (30 min)
        var verificationCode = GenerateVerificationCode();
        var verificationTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        // Création de l'entité Client avec mappage complet des champs de la requête
        var newClient = new Client
        {
            // Identifiants techniques et sécurité
            Code = $"CLT-{DateTime.UtcNow.Ticks}",
            MotPasse = hashedPassword,

            // Informations principales
            RaisonSociale = request.RaisonSociale,
            Email = request.Email,

            // Informations de contact
            ContactNom = request.ContactNom,
            ContactTelephone = request.ContactTelephone,
            ContactFonction = request.ContactFonction,

            // Informations société et adresse
            TypeClient = request.TypeClient,
            RegistreCommerce = request.RegistreCommerce,
            Adresse = request.Adresse,
            Bp = request.Bp,
            Ville = request.Ville,
            Pays = request.Pays,

            // Statuts par défaut
            Desactive = 0,
            ChangementMotPasse = 1,

            // Workflow de validation (État initial)
            IsVerified = false,
            NiveauValidation = 0,
            VerificationCode = verificationCode,
            VerificationTokenExpiry = verificationTokenExpiry,

            // Audit
            DateCreation = DateTime.UtcNow,
            UtilisateurCreation = "SYSTEM_REGISTER"
        };

        // Persistance dans la base de données
        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync(cancellationToken);

        // Génération du token de vérification (JWT temporaire).
        // Ce token servira uniquement à identifier l'utilisateur lors de l'appel à l'endpoint de confirmation OTP.
        var verificationToken = _jwtTokenGenerator.GenerateToken(newClient.Id, newClient.Email);

        // Envoi de l'e-mail contenant le code OTP au client
        await SendVerificationEmailAsync(newClient.Email, verificationCode);

        // Envoi d'une notification par e-mail à l'administration de l'ARPCE
        await SendNotificationToArpceAsync(newClient);

        // Retourne le résultat
        return new AuthenticationResult
        {
            Message = "Inscription enregistrée avec succès. Veuillez vérifier votre e-mail pour récupérer votre code de confirmation.",
            Token = verificationToken // Token restreint pour la confirmation
        };
    }

    /// <summary>
    /// Gère le cas où un utilisateur tente de se réinscrire alors que son compte n'est pas encore validé par OTP.
    /// </summary>
    private async Task<AuthenticationResult> ResendVerificationAsync(Client client, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Demande de renvoi de code OTP pour le client {Email}", client.Email);

        // Régénération d'un nouveau code et extension de la durée de validité
        client.VerificationCode = GenerateVerificationCode();
        client.VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        // Mise à jour en base
        await _context.SaveChangesAsync(cancellationToken);

        // Génération d'un nouveau token de vérification
        var verificationToken = _jwtTokenGenerator.GenerateToken(client.Id, client.Email!);

        // Renvoi de l'e-mail au client
        await SendVerificationEmailAsync(client.Email!, client.VerificationCode);

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
        var body = $@"
            <div style='font-family: Arial, sans-serif;'>
                <h1>Bienvenue sur la plateforme d'homologation de l'ARPCE</h1>
                <p>Merci de votre inscription. Pour valider votre adresse e-mail, veuillez utiliser le code ci-dessous :</p>
                <h2 style='color: #2c3e50; letter-spacing: 5px;'>{code}</h2>
                <p>Ce code est valable pendant <strong>30 minutes</strong>.</p>
                <p>Si vous n'êtes pas à l'origine de cette demande, veuillez ignorer cet e-mail.</p>
            </div>";

        await _emailService.SendEmailAsync(email, subject, body);
    }

    /// <summary>
    /// Envoie une notification à l'administrateur ARPCE pour l'informer d'une nouvelle inscription.
    /// </summary>
    private async Task SendNotificationToArpceAsync(Client client)
    {
        // Récupère l'email de notification admin depuis la configuration
        var arpceEmail = _configuration["EmailSettings:NotificationEmail"];

        if (string.IsNullOrEmpty(arpceEmail))
        {
            _logger.LogWarning("Email de notification ARPCE (EmailSettings:NotificationEmail) non configuré. La notification n'a pas été envoyée.");
            return;
        }

        var subject = $"[NOUVELLE INSCRIPTION] - {client.RaisonSociale}";
        var body = $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2>Nouvelle demande d'inscription Redevable</h2>
                <p>Un nouveau compte a été créé et est en attente de validation OTP.</p>
                <hr />
                <ul>
                    <li><strong>Raison Sociale :</strong> {client.RaisonSociale}</li>
                    <li><strong>Type Client :</strong> {client.TypeClient}</li>
                    <li><strong>Contact :</strong> {client.ContactNom}</li>
                    <li><strong>Email :</strong> {client.Email}</li>
                    <li><strong>Téléphone :</strong> {client.ContactTelephone}</li>
                    <li><strong>Ville/Pays :</strong> {client.Ville}, {client.Pays}</li>
                </ul>
                <hr />
                <p>Ce compte passera au statut 'En attente de validation ARPCE' une fois l'OTP saisi par le client.</p>
            </div>";

        try
        {
            // Envoi de l'e-mail admin
            await _emailService.SendEmailAsync(arpceEmail, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de la notification d'inscription à l'adresse admin {AdminEmail}.", arpceEmail);
        }
    }
}