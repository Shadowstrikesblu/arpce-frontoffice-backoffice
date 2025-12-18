using System.Threading.Tasks;

namespace FrontOffice.Application.Common.Interfaces;

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "XAF";
    public string ExternalId { get; set; }
    public string PayerPhoneNumber { get; set; }
    public string PayerMessage { get; set; }
}

/// <summary>
/// Interface générique pour tous les services de paiement mobile.
/// Chaque opérateur (MTN, Airtel) aura sa propre implémentation.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Le code unique du provider (ex: "mtn", "airtel").
    /// </summary>
    string ProviderCode { get; }

    Task<string> GetAccessTokenAsync();
    Task<string> RequestPaymentAsync(PaymentRequest request, string accessToken);
    Task<string> GetTransactionStatusAsync(string transactionId, string accessToken);
}