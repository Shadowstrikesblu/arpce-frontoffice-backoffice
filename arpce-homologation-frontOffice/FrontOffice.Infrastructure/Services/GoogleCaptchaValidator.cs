using FrontOffice.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontOffice.Infrastructure.Services;

public class GoogleCaptchaValidator : ICaptchaValidator
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleCaptchaValidator> _logger;

    public GoogleCaptchaValidator(IConfiguration configuration, HttpClient httpClient, ILogger<GoogleCaptchaValidator> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ValidateAsync(string captchaToken)
    {
        // Pour la sécu, on bloque si pas de token.
        if (string.IsNullOrWhiteSpace(captchaToken))
        {
            _logger.LogWarning("Validation Captcha échouée : Token manquant.");
            return false;
        }

        var secretKey = _configuration["CaptchaSettings:SecretKey"];
        var verifyUrl = _configuration["CaptchaSettings:VerifyUrl"];

        if (string.IsNullOrEmpty(secretKey))
        {
            _logger.LogError("Clé secrète Captcha manquante dans la configuration.");
            return false;
        }

        try
        {
            var response = await _httpClient.PostAsync($"{verifyUrl}?secret={secretKey}&response={captchaToken}", null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Appel API Google Captcha échoué. Status: {Status}", response.StatusCode);
                return false;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var googleResponse = JsonSerializer.Deserialize<GoogleCaptchaResponse>(jsonString);

            if (googleResponse != null && !googleResponse.Success)
            {
                _logger.LogWarning("Validation Captcha Google échouée. Erreurs: {Errors}", string.Join(", ", googleResponse.ErrorCodes ?? Array.Empty<string>()));
            }

            return googleResponse?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du Captcha.");
            return false;
        }
    }

    // Classe interne pour désérialiser la réponse Google
    private class GoogleCaptchaResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("success")]
        public bool Success { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}