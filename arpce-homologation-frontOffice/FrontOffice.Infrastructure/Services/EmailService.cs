using FrontOffice.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace FrontOffice.Infrastructure.Services;

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

        ArgumentNullException.ThrowIfNullOrWhiteSpace(smtpServer, "EmailSettings:SmtpServer");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(portString, "EmailSettings:Port");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(senderName, "EmailSettings:SenderName");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(senderEmail, "EmailSettings:SenderEmail");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(password, "EmailSettings:Password");
        // --------------------------------------------------------------------------

        // Conversion du port en entier après avoir vérifié qu'il n'est pas null
        var port = int.Parse(portString);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(senderEmail, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}