using FrontOffice.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace FrontOffice.Infrastructure.Services;

public class AirtelPaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AirtelPaymentService> _logger;

    public string ProviderCode => "airtel";

    public AirtelPaymentService(IConfiguration configuration, HttpClient httpClient, ILogger<AirtelPaymentService> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    // Obtention du token (OAuth2)
    public async Task<string> GetAccessTokenAsync()
    {
        var baseUrl = _configuration["AirtelApiSettings:BaseUrl"];
        var clientId = _configuration["AirtelApiSettings:ClientId"];
        var clientSecret = _configuration["AirtelApiSettings:ClientSecret"];
        var grantType = _configuration["AirtelApiSettings:GrantType"];

        var requestUri = $"{baseUrl}/auth/oauth2/token";

        var payload = new
        {
            client_id = clientId,
            client_secret = clientSecret,
            grant_type = grantType
        };

        var response = await _httpClient.PostAsync(requestUri,
            new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Échec obtention token Airtel. Statut: {s}, Réponse: {r}", response.StatusCode, responseContent);
            throw new Exception("Impossible d'obtenir le token Airtel.");
        }

        var tokenResponse = JsonConvert.DeserializeObject<AirtelTokenResponse>(responseContent);
        return tokenResponse?.AccessToken ?? throw new Exception("Token Airtel invalide.");
    }

    // Demande un paiement 
    public async Task<string> RequestPaymentAsync(PaymentRequest requestData, string accessToken)
    {
        // (Exemple : https://openapiuat.airtel.cg/merchant/v1/payments/)
        _logger.LogInformation("Logique RequestPaymentAsync pour Airtel à implémenter.");
        // Simule pour l'instant
        return await Task.FromResult(Guid.NewGuid().ToString());
    }

    // Vérifie le statut
    public async Task<string> GetTransactionStatusAsync(string transactionId, string accessToken)
    {
        // A adapter avec la documentation Airtel
        _logger.LogInformation("Logique GetTransactionStatusAsync pour Airtel à implémenter.");
        return await Task.FromResult("SUCCESSFUL");
    }

    private class AirtelTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}