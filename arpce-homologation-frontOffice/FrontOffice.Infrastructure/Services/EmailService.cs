using FrontOffice.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace FrontOffice.Infrastructure.Services;

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
        var message = new MimeKit.MimeMessage();
        var from = _configuration["EmailSettings:SmtpFrom"];
        var senderName = _configuration["EmailSettings:SenderName"];

        message.From.Add(new MailboxAddress(senderName, from));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                _configuration["EmailSettings:SmtpHost"],
                int.Parse(_configuration["EmailSettings:SmtpPort"]),
                SecureSocketOptions.SslOnConnect);

            await client.AuthenticateAsync(
                _configuration["EmailSettings:SmtpUser"],
                _configuration["EmailSettings:SmtpPass"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email envoyé à {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur SMTP Hostinger pour {Email}", toEmail);
            throw new InvalidOperationException("Le service d'envoi d'emails a rencontré une erreur.");
        }
    }
}