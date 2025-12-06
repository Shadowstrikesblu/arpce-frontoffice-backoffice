using Newtonsoft.Json;

namespace FrontOffice.Application.Features.Paiements.Commands.HandleMomoWebhook;

public class MomoWebhookPayload
{
    [JsonProperty("externalId")]
    public string ExternalId { get; set; } = string.Empty; 

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty; // "SUCCESSFUL" ou "FAILED"

    [JsonProperty("financialTransactionId")]
    public string FinancialTransactionId { get; set; } = string.Empty; // L'ID de MTN

    public Payer Payer { get; set; } = new Payer();
}

public class Payer
{
    [JsonProperty("partyIdType")]
    public string PartyIdType { get; set; } = string.Empty;

    [JsonProperty("partyId")]
    public string PartyId { get; set; } = string.Empty; // Le numéro de téléphone
}