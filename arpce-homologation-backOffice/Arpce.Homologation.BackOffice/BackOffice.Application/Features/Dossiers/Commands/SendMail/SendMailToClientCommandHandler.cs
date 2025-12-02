using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Commands.SendMail;

public class SendMailToClientCommandHandler : IRequestHandler<SendMailToClientCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public SendMailToClientCommandHandler(IApplicationDbContext context, IEmailService emailService, IAuditService auditService)
    {
        _context = context;
        _emailService = emailService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(SendMailToClientCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null) throw new Exception("Dossier introuvable.");
        if (dossier.Client?.Email == null) throw new Exception("Le client n'a pas d'adresse e-mail.");

        string subject = "";
        string body = "";

        switch (request.Type)
        {
            case "RappelPaiement":
                subject = $"Rappel de Paiement - Dossier {dossier.Numero}";
                body = $"<h1>Rappel</h1><p>Bonjour, le paiement pour votre dossier {dossier.Numero} est en attente.</p>";
                break;
            // Ajouter d'autres cas ici
            default:
                throw new Exception($"Type de mail '{request.Type}' non supporté.");
        }

        await _emailService.SendEmailAsync(dossier.Client.Email, subject, body);

        await _auditService.LogAsync(
    page: "Communication Client",
    libelle: $"Envoi d'un e-mail de type '{request.Type}' pour le dossier '{dossier.Numero}'.",
    eventTypeCode: "COMMUNICATION",
    dossierId: dossier.Id);
        return true;
    }
}