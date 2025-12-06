using FrontOffice.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Infrastructure.Services;

public class MomoPaymentService : IMomoPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MomoPaymentService> _logger;

    public MomoPaymentService(IConfiguration configuration, HttpClient httpClient, ILogger<MomoPaymentService> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Étape 1 : Obtenir un token d'accès auprès de l'API MTN.
    /// </summary>
    public async Task<string> GetAccessTokenAsync()
    {
        var baseUrl = _configuration["MomoApiSettings:BaseUrl"];
        var subscriptionKey = _configuration["MomoApiSettings:SubscriptionKey"];
        var apiUser = _configuration["MomoApiSettings:ApiUser"];
        var apiKey = _configuration["MomoApiSettings:ApiKey"];

        var requestUri = $"{baseUrl}/collection/token/";
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiUser}:{apiKey}"));

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<MomoTokenResponse>(jsonString);

        if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
        {
            throw new Exception("Impossible d'obtenir le token d'accès MTN MoMo.");
        }

        return tokenResponse.AccessToken;
    }


    /// <summary>
    /// Étape 2 : Initier une demande de paiement.
    /// </summary>
    public async Task<string> RequestPaymentAsync(MomoPaymentRequest requestData, string accessToken)
    {
        var baseUrl = _configuration["MomoApiSettings:BaseUrl"];
        var subscriptionKey = _configuration["MomoApiSettings:SubscriptionKey"];
        var callbackUrl = _configuration["MomoApiSettings:CallbackUrl"];

        var transactionId = Guid.NewGuid().ToString();
        var requestUri = $"{baseUrl}/collection/v1_0/requesttopay";

        var payload = new
        {
            amount = requestData.Amount.ToString(),
            currency = requestData.Currency,
            externalId = requestData.ExternalId,
            payer = new { partyIdType = "MSISDN", partyId = requestData.PayerPhoneNumber },
            payerMessage = requestData.PayerMessage,
            payeeNote = "Paiement frais ARPCE"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("X-Reference-Id", transactionId);
        request.Headers.Add("X-Target-Environment", "sandbox"); // Enleve ou changer pour "mtncongo" en prod
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        //L'URL de callback doit être accessible publiquement sur Internet
        if (!string.IsNullOrEmpty(callbackUrl))
        {
            request.Headers.Add("X-Callback-Url", callbackUrl);
        }

        var response = await _httpClient.SendAsync(request);

        // L'API MTN retourne 202 Accepted pour une demande initiée avec succès
        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Échec de la demande de paiement MTN MoMo. Statut: {status}, Contenu: {content}", response.StatusCode, errorContent);
            throw new Exception("La demande de paiement auprès de MTN a échoué.");
        }

        return transactionId;
    }

    public async Task<string> GetTransactionStatusAsync(string transactionReferenceId, string accessToken)
    {
        var baseUrl = _configuration["MomoApiSettings:BaseUrl"];
        var subscriptionKey = _configuration["MomoApiSettings:SubscriptionKey"];

        var requestUri = $"{baseUrl}/collection/v1_0/requesttopay/{transactionReferenceId}";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        request.Headers.Add("X-Target-Environment", "sandbox");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        return jsonString;
    }

    private class MomoTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}