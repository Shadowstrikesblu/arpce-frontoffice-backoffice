using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrontOffice.Application.Features.Paiements.Commands.HandleMomoWebhook;

public class HandleMomoWebhookCommand : IRequest
{
    public MomoWebhookPayload Payload { get; set; }

    public HandleMomoWebhookCommand(MomoWebhookPayload payload) => Payload = payload;
}

public class HandleMomoWebhookCommandHandler : IRequestHandler<HandleMomoWebhookCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<HandleMomoWebhookCommandHandler> _logger;

    public HandleMomoWebhookCommandHandler(IApplicationDbContext context, ILogger<HandleMomoWebhookCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(HandleMomoWebhookCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        _logger.LogInformation("Webhook MTN reçu pour ExternalId {ExternalId} avec statut {Status}", payload.ExternalId, payload.Status);

        if (!Guid.TryParse(payload.ExternalId, out var devisId))
        {
            _logger.LogWarning("ID externe du webhook MTN invalide : {ExternalId}", payload.ExternalId);
            return; // On ignore la requête
        }

        // On charge le devis et ses parents (Demande -> Dossier) pour mettre à jour les statuts
        var devis = await _context.Devis
            .Include(d => d.Demande)
                .ThenInclude(dem => dem.Dossier)
            .FirstOrDefaultAsync(d => d.Id == devisId, cancellationToken);

        if (devis == null)
        {
            _logger.LogWarning("Devis avec ID {DevisId} non trouvé suite à la notification webhook.", devisId);
            return;
        }

        if (payload.Status == "SUCCESSFUL")
        {
            // Paiement réussi
            devis.PaiementOk = 1; // 1 = Payé

            // Met à jour le statut du dossier à "Paiement Effectué"
            var statutPaye = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.DossierPayer.ToString(), cancellationToken);
            if (statutPaye != null)
            {
                devis.Demande.Dossier.IdStatut = statutPaye.Id;
            }

            _logger.LogInformation("Paiement MTN validé pour le devis {DevisId}, dossier {DossierId}", devisId, devis.Demande.Dossier.Id);
        }
        else 
        {
            // Paiement échoué
            // Met à jour le statut du dossier à "Paiement Rejeté"
            var statutRejete = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.PaiementRejete.ToString(), cancellationToken);
            if (statutRejete != null)
            {
                devis.Demande.Dossier.IdStatut = statutRejete.Id;
            }

            _logger.LogWarning("Paiement MTN échoué pour le devis {DevisId}", devisId);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}