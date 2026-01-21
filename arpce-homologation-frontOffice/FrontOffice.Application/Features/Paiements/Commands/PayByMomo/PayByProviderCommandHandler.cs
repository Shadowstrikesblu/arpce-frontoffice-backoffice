using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Paiements.Commands.PayByMomo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Paiements.Commands.PayByProvider;

public class PayByProviderCommandHandler : IRequestHandler<PayByProviderCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly Func<string, IPaymentService> _paymentServiceFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService; 

    public PayByProviderCommandHandler(
        IApplicationDbContext context,
        Func<string, IPaymentService> paymentServiceFactory,
        ICurrentUserService currentUserService,
        INotificationService notificationService) 
    {
        _context = context;
        _paymentServiceFactory = paymentServiceFactory;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(PayByProviderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Non authentifié.");

        var dossierAvecDevis = await _context.Dossiers
            .Where(d => d.Id == request.DossierId && d.IdClient == userId.Value)
            .Select(d => new {
                DossierNumero = d.Numero,
                Devis = d.Devis.FirstOrDefault(dev => dev.PaiementOk != 1)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dossierAvecDevis?.Devis == null) throw new InvalidOperationException("Aucun devis en attente trouvé.");

        var devis = dossierAvecDevis.Devis;

        var paymentService = _paymentServiceFactory(request.Provider);
        var amount = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0);
        var accessToken = await paymentService.GetAccessTokenAsync();

        var paymentRequest = new PaymentRequest
        {
            Amount = amount,
            ExternalId = devis.Id.ToString(),
            PayerPhoneNumber = request.PhoneNumber,
            PayerMessage = $"Paiement frais homologation pour dossier {dossierAvecDevis.DossierNumero}"
        };

        var transactionId = await paymentService.RequestPaymentAsync(paymentRequest, accessToken);

        var devisToUpdate = await _context.Devis.FindAsync(new object[] { devis.Id }, cancellationToken);
        if (devisToUpdate != null)
        {
            devisToUpdate.PaiementMobileId = transactionId;
            await _context.SaveChangesAsync(cancellationToken);
        }

        await _notificationService.SendToGroupAsync(
            groupName: "DAFC",
            title: "Tentative de Paiement",
            message: $"Une tentative de paiement via {request.Provider.ToUpper()} a été initiée pour le dossier {dossierAvecDevis.DossierNumero}.",
            type: "Info"
        );

        return true;
    }
}