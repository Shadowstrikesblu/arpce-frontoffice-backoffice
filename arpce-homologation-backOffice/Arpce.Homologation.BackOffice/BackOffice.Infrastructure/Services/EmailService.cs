using BackOffice.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

/// <summary>
/// Implémentation du service d'envoi d'e-mails via SMTP avec MailKit.
/// Conçu pour être compatible avec différents serveurs SMTP (Gmail, SendGrid, Exchange...).
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var portString = _configuration["EmailSettings:Port"];
        var senderName = _configuration["EmailSettings:SenderName"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];

        if (string.IsNullOrWhiteSpace(smtpServer) || string.IsNullOrWhiteSpace(portString) ||
            string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            _logger.LogError("Configuration SMTP (EmailSettings) incomplète. L'email pour '{ToEmail}' n'a pas été envoyé.", toEmail);
            return;
        }

        if (!int.TryParse(portString, out var port))
        {
            _logger.LogError("Le port SMTP '{PortString}' est invalide.", portString);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

        try
        {
            SecureSocketOptions options = port switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.Auto,
            };

            _logger.LogInformation("Connexion au serveur SMTP {SmtpServer} sur le port {Port} avec l'option {SecureOption}...", smtpServer, port, options);
            await client.ConnectAsync(smtpServer, port, options);

            _logger.LogInformation("Authentification avec l'utilisateur '{Username}'...", username);
            await client.AuthenticateAsync(username, password);

            _logger.LogInformation("Envoi de l'email à '{ToEmail}'...", toEmail);
            await client.SendAsync(message);

            await client.DisconnectAsync(true);
            _logger.LogInformation("Email envoyé avec succès.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Échec de l'envoi de l'email à {ToEmail}. Erreur : {ErrorMessage}", toEmail, ex.Message);
            throw new InvalidOperationException("Le service d'envoi d'emails a rencontré une erreur.", ex);
        }
    }
}