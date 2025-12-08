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

/// <summary>
/// Implémentation du service pour interagir avec l'API MTN Mobile Money (MoMo).
/// Gère l'authentification, l'initiation de paiement et la vérification de statut.
/// </summary>
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
    /// Étape 1 : Obtenir un token d'accès temporaire auprès de l'API MTN.
    /// Ce token est nécessaire pour toutes les autres requêtes.
    /// </summary>
    public async Task<string> GetAccessTokenAsync()
    {
        // Lecture des paramètres de configuration
        var baseUrl = _configuration["MomoApiSettings:BaseUrl"];
        var subscriptionKey = _configuration["MomoApiSettings:SubscriptionKey"];
        var apiUser = _configuration["MomoApiSettings:ApiUser"];
        var apiKey = _configuration["MomoApiSettings:ApiKey"];

        // Construction de la requête pour obtenir le token
        var requestUri = $"{baseUrl}/collection/token/";
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiUser}:{apiKey}"));

        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
            // L'authentification pour le token se fait en "Basic"
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // La clé de souscription est requise par la gateway API de MTN
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            _logger.LogInformation("Demande de token d'accès MTN à l'URL : {RequestUri}", requestUri);

            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Échec de l'obtention du token MTN. Statut: {StatusCode}, Réponse: {Response}", response.StatusCode, responseContent);
                throw new Exception("Impossible d'obtenir le token d'accès MTN MoMo.");
            }

            var tokenResponse = JsonConvert.DeserializeObject<MomoTokenResponse>(responseContent);

            if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
            {
                _logger.LogError("La réponse pour le token MTN ne contient pas de access_token valide.");
                throw new Exception("Réponse invalide de l'API de tokenisation MTN MoMo.");
            }

            _logger.LogInformation("Token d'accès MTN obtenu avec succès.");
            return tokenResponse.AccessToken;
        }
    }

    /// <summary>
    /// Étape 2 : Initier une demande de paiement ("Request to Pay").
    /// L'utilisateur recevra une notification sur son téléphone pour approuver la transaction.
    /// </summary>
    /// <param name="requestData">Les détails de la transaction (montant, numéro, etc.).</param>
    /// <param name="accessToken">Le token d'accès obtenu à l'étape 1.</param>
    /// <returns>L'identifiant de référence de la transaction (X-Reference-Id) pour un suivi ultérieur.</returns>
    public async Task<string> RequestPaymentAsync(MomoPaymentRequest requestData, string accessToken)
    {
        var baseUrl = _configuration["MomoApiSettings:BaseUrl"];
        var subscriptionKey = _configuration["MomoApiSettings:SubscriptionKey"];
        var callbackUrl = _configuration["MomoApiSettings:CallbackUrl"];

        // Un ID de transaction unique que nous générons pour cette requête
        var transactionId = Guid.NewGuid().ToString();
        var requestUri = $"{baseUrl}/collection/v1_0/requesttopay";

        var payload = new
        {
            amount = requestData.Amount.ToString("F0"), // Format sans décimales si requis
            currency = requestData.Currency,
            externalId = requestData.ExternalId,
            payer = new { partyIdType = "MSISDN", partyId = requestData.PayerPhoneNumber },
            payerMessage = requestData.PayerMessage,
            payeeNote = "Paiement frais homologation ARPCE"
        };

        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // L'authentification pour cette requête se fait avec le token "Bearer"
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("X-Reference-Id", transactionId);
            request.Headers.Add("X-Target-Environment", "sandbox"); // À changer/enlever pour la production
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Si une URL de webhook est configurée, on l'ajoute à la requête
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                request.Headers.Add("X-Callback-Url", callbackUrl);
            }

            _logger.LogInformation("Initiation du paiement MoMo pour ExternalId {ExternalId}", requestData.ExternalId);

            var response = await _httpClient.SendAsync(request);

            // L'API MTN retourne 202 Accepted pour une demande initiée avec succès
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Échec de la demande de paiement MTN MoMo. Statut: {status}, Contenu: {content}", response.StatusCode, errorContent);
                //throw new Exception("La demande de paiement auprès de l'opérateur a échoué.");
            }

            _logger.LogInformation("Demande de paiement MoMo pour ExternalId {ExternalId} initiée avec succès. ReferenceId: {TransactionId}", requestData.ExternalId, transactionId);

            return transactionId;
        }
    }

    /// <summary>
    /// Étape 3 : Vérifier le statut d'une transaction initiée (Polling).
    /// </summary>
    /// <param name="transactionReferenceId">L'ID de référence (X-Reference-Id) retourné par RequestPaymentAsync.</param>
    /// <param name="accessToken">Le token d'accès.</param>
    /// <returns>Le statut de la transaction (ex: "SUCCESSFUL", "PENDING", "FAILED").</returns>
    public async Task<string> GetTransactionStatusAsync(string transactionReferenceId, string accessToken)
    {
        var baseUrl = _configuration["MomoApiSettings:BaseUrl"];
        var subscriptionKey = _configuration["MomoApiSettings:SubscriptionKey"];

        var requestUri = $"{baseUrl}/collection/v1_0/requesttopay/{transactionReferenceId}";

        using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("X-Target-Environment", "sandbox");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var statusResponse = JsonConvert.DeserializeObject<MomoStatusResponse>(jsonString);

            return statusResponse?.Status ?? "UNKNOWN";
        }
    }

    // Classe privée pour désérialiser la réponse du token
    private class MomoTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }

    // Classe privée pour désérialiser la réponse de statut
    private class MomoStatusResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}