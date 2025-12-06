namespace FrontOffice.Application.Common.Interfaces;

public class MomoPaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "XAF"; 
    public string ExternalId { get; set; } 
    public string PayerPhoneNumber { get; set; }
    public string PayerMessage { get; set; } = "Paiement frais homologation";
}

public interface IMomoPaymentService
{
    // Obtient un token d'accès auprès de l'API MTN
    Task<string> GetAccessTokenAsync();

    // Demande un paiement au client
    // Retourne l'ID de la transaction MTN
    Task<string> RequestPaymentAsync(MomoPaymentRequest request, string accessToken);

    // Vérifie le statut d'une transaction
    Task<string> GetTransactionStatusAsync(string transactionId, string accessToken);

}