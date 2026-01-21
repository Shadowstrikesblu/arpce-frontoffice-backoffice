using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Paiements.Commands.HandleMomoWebhook;

public class HandleMomoWebhookCommandHandler : IRequestHandler<HandleMomoWebhookCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<HandleMomoWebhookCommandHandler> _logger;
    private readonly INotificationService _notificationService; 

    public HandleMomoWebhookCommandHandler(
        IApplicationDbContext context,
        ILogger<HandleMomoWebhookCommandHandler> logger,
        INotificationService notificationService) 
    {
        _context = context;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Handle(HandleMomoWebhookCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        _logger.LogInformation("Webhook reçu pour ExternalId {Id} avec statut {Status}", payload.ExternalId, payload.Status);

        if (!Guid.TryParse(payload.ExternalId, out var devisId))
        {
            _logger.LogWarning("ID externe du webhook invalide : {Id}", payload.ExternalId);
            return;
        }

        var devis = await _context.Devis
            .Include(d => d.Dossier)
            .FirstOrDefaultAsync(d => d.Id == devisId, cancellationToken);

        if (devis == null)
        {
            _logger.LogWarning("Devis {Id} non trouvé via webhook.", devisId);
            return;
        }

        if (payload.Status == "SUCCESSFUL")
        {
            devis.PaiementOk = 1;
            var statutPaye = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.DossierPayer.ToString(), cancellationToken);
            if (statutPaye != null) devis.Dossier.IdStatut = statutPaye.Id;

            _logger.LogInformation("Paiement validé pour le devis {Id}", devisId);

            await _notificationService.SendToGroupAsync(
                groupName: "DAFC",
                title: "Paiement Réussi",
                message: $"Le paiement pour le dossier {devis.Dossier.Numero} a été effectué avec succès.",
                type: "Success",
                targetUrl: $"/dossiers/{devis.Dossier.Id}"
            );
        }
        else
        {
            var statutRejete = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.PaiementRejete.ToString(), cancellationToken);
            if (statutRejete != null) devis.Dossier.IdStatut = statutRejete.Id;

            _logger.LogWarning("Paiement échoué pour le devis {Id}", devisId);

            await _notificationService.SendToGroupAsync(
                groupName: "DAFC",
                title: "Paiement Échoué",
                message: $"Le paiement pour le dossier {devis.Dossier.Numero} a échoué.",
                type: "Error",
                targetUrl: $"/dossiers/{devis.Dossier.Id}"
            );
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}