using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Application.Features.Public.Commands.SendContactMail;

public class SendContactMailCommandHandler : IRequestHandler<SendContactMailCommand, bool>
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public SendContactMailCommandHandler(IEmailService emailService, IConfiguration configuration)
    {
        emailService = _emailService;
        _configuration = configuration;
    }

    public async Task<bool> Handle(SendContactMailCommand request, CancellationToken cancellationToken)
    {
        var arpceEmail = _configuration["EmailSettings:NotificationEmail"];
        if (string.IsNullOrEmpty(arpceEmail)) return false;

        var subject = $"[Contact FO] - {request.Subject}";
        var body = $@"
            <p>Message reçu depuis le formulaire de contact du Front Office :</p>
            <ul>
                <li><strong>Nom :</strong> {request.Name}</li>
                <li><strong>Email :</strong> {request.Email}</li>
            </ul>
            <hr>
            <p><strong>Message :</strong></p>
            <p>{request.Message}</p>";

        await _emailService.SendEmailAsync(arpceEmail, subject, body);
        return true;
    }
}