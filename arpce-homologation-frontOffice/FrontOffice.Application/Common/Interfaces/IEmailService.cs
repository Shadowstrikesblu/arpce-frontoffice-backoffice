namespace FrontOffice.Application.Common.Interfaces;

/// <summary>
/// Définit un contrat pour un service d'envoi d'e-mails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envoie un e-mail de manière asynchrone.
    /// </summary>
    /// <param name="toEmail">L'adresse e-mail du destinataire.</param>
    /// <param name="subject">Le sujet de l'e-mail.</param>
    /// <param name="body">Le corps de l'e-mail (peut être du HTML).</param>
    Task SendEmailAsync(string toEmail, string subject, string body);
}