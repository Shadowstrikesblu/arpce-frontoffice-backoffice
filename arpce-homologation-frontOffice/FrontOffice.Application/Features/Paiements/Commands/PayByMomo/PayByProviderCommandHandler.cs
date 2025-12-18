using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Paiements.Commands.PayByMomo;

public class PayByProviderCommandHandler : IRequestHandler<PayByProviderCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly Func<string, IPaymentService> _paymentServiceFactory; 
    private readonly ICurrentUserService _currentUserService;

    public PayByProviderCommandHandler(
        IApplicationDbContext context,
        Func<string, IPaymentService> paymentServiceFactory,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _paymentServiceFactory = paymentServiceFactory;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(PayByProviderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Non authentifié.");

        // Récupére le devis
        var devis = await _context.Dossiers
            .Where(d => d.Id == request.DossierId && d.IdClient == userId.Value)
            .Select(d => d.Demandes.SelectMany(dem => dem.Devis).FirstOrDefault(dev => dev.PaiementOk != 1))
            .FirstOrDefaultAsync(cancellationToken);

        if (devis == null) throw new InvalidOperationException("Aucun devis en attente trouvé.");

        // SÉLECTIONNE LE BON SERVICE GRÂCE À LA FACTORY
        var paymentService = _paymentServiceFactory(request.Provider);

        var amount = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0);

        // Appele le service sélectionné
        var accessToken = await paymentService.GetAccessTokenAsync();

        var paymentRequest = new PaymentRequest
        {
            Amount = amount,
            ExternalId = devis.Id.ToString(),
            PayerPhoneNumber = request.PhoneNumber,
            PayerMessage = $"Paiement frais homologation"
        };

        var transactionId = await paymentService.RequestPaymentAsync(paymentRequest, accessToken);

        // Mise à jour du devis (logique inchangée)
        var devisToUpdate = await _context.Devis.FindAsync(devis.Id);
        if (devisToUpdate != null)
        {
            devisToUpdate.PaiementMobileId = transactionId;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}