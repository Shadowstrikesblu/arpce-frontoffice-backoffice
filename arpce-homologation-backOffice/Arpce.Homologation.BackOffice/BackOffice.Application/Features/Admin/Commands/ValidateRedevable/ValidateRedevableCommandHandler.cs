using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.ValidateRedevable;

public class ValidateRedevableCommandHandler : IRequestHandler<ValidateRedevableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public ValidateRedevableCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _emailService = emailService;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(ValidateRedevableCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(new object[] { request.RedevableId }, cancellationToken);

        if (client == null)
        {
            throw new Exception($"Redevable avec l'ID '{request.RedevableId}' introuvable.");
        }

        if (client.NiveauValidation >= 2)
        {
            return true;
        }

        client.NiveauValidation = 2; 
        client.Desactive = 0; 

        await _context.SaveChangesAsync(cancellationToken);

        // Email
        if (!string.IsNullOrEmpty(client.Email))
        {
            var subject = "Validation de votre compte ARPCE Homologation";
            var body = $@"<h1>Félicitations !</h1><p>Votre compte <strong>{client.RaisonSociale}</strong> a été validé.</p>";

            try
            {
                await _emailService.SendEmailAsync(client.Email, subject, body);
            }
            catch { }
        }

        await _auditService.LogAsync(
            page: "Validation Redevable",
            libelle: $"Le compte du redevable '{client.RaisonSociale}' (ID: {client.Id}) a été validé (Niveau 2).",
            eventTypeCode: "VALIDATION");

        await _notificationService.SendToGroupAsync(
            profilCode: "UTILISATEURS", 
            title: "Nouveau Redevable Actif",
            message: $"Le compte {client.RaisonSociale} a été validé.",
            type: "E", 
            targetUrl: $"/redevables/{client.Id}",
            entityId: client.Id.ToString()
        );

        return true;
    }
}