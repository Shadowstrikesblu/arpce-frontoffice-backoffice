// Fichier : BackOffice.Infrastructure/Services/EmailService.cs

using BackOffice.Application.Common.Interfaces;
using MailKit.Net.Smtp; // IMPORTANT : Utiliser MailKit
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var portString = _configuration["EmailSettings:Port"];
        var senderName = _configuration["EmailSettings:SenderName"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var password = _configuration["EmailSettings:Password"];

        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(portString) ||
            string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password))
        {
            throw new Exception("Configuration SMTP incomplète (Vérifiez appsettings.json et User Secrets).");
        }

        var port = int.Parse(portString);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            // Accepte tous les certificats SSL (utile pour le dev/debug, à sécuriser en prod si besoin)
            client.CheckCertificateRevocation = false;

            await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(senderEmail, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}