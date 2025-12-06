using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Paiements.Commands.PayByMomo;

public class PayByMomoCommandHandler : IRequestHandler<PayByMomoCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMomoPaymentService _momoService;
    private readonly ICurrentUserService _currentUserService;

    public PayByMomoCommandHandler(
        IApplicationDbContext context,
        IMomoPaymentService momoService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _momoService = momoService;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(PayByMomoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // 1. Récupérer le dossier, sa demande et le premier devis non payé.
        var dossierAvecDevis = await _context.Dossiers
            .Where(d => d.Id == request.DossierId && d.IdClient == userId.Value)
            .Select(d => new
            {
                Dossier = d,
                Devis = d.Demandes
                         .SelectMany(dem => dem.Devis) 
                         .FirstOrDefault(dev => dev.PaiementOk != 1)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dossierAvecDevis?.Devis == null)
        {
            throw new InvalidOperationException("Aucun devis en attente de paiement trouvé pour ce dossier.");
        }

        var devis = dossierAvecDevis.Devis;
        var dossier = dossierAvecDevis.Dossier;

        // Calcul du montant total du devis
        var amount = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0);

        // Obtient le token d'accès MTN
        var accessToken = await _momoService.GetAccessTokenAsync();

        // Prépare la requête de paiement
        var paymentRequest = new MomoPaymentRequest
        {
            Amount = amount,
            ExternalId = devis.Id.ToString(),
            PayerPhoneNumber = request.PhoneNumber,
            PayerMessage = $"Paiement frais homologation pour dossier {dossier.Numero}"
        };

        // Initie le paiement via le service MoMo
        var transactionId = await _momoService.RequestPaymentAsync(paymentRequest, accessToken);

        // Met à jour notre devis avec l'ID de la transaction MTN pour le suivi

        // Il faut récupérer l'entité Devis trackée pour la mettre à jour
        var devisToUpdate = await _context.Devis.FindAsync(new object[] { devis.Id }, cancellationToken);
        if (devisToUpdate != null)
        {
            devisToUpdate.PaiementMobileId = transactionId;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return true; 
    }
}