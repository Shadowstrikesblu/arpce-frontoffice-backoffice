using System.Net;
using System.Text.Json;

namespace BackOffice.Api.Middleware;

/// <summary>
/// Middleware pour intercepter les exceptions non gérées dans le pipeline de requêtes
/// et les transformer en une réponse HTTP JSON standardisée.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Tente d'exécuter la suite du pipeline.
            await _next(context);
        }
        catch (Exception ex)
        {
            // Si une exception est levée, on la gère ici.
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // On détermine le code de statut HTTP et le message en fonction du type d'exception.
        var (statusCode, message) = exception switch
        {
            // Cas spécifique pour les erreurs d'autorisation.
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message),

            // Cas spécifique pour les erreurs de validation métier (exemple).
            InvalidOperationException => (HttpStatusCode.UnprocessableEntity, exception.Message), // 422 Unprocessable Entity

            // Cas par défaut pour toutes les autres exceptions non prévues.
            _ => (HttpStatusCode.InternalServerError, "Une erreur interne est survenue.")
        };

        // Si c'est une erreur interne inattendue, on la logue avec tous les détails.
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Une exception non gérée a été interceptée par le middleware.");
        }

        // Préparation de la réponse JSON.
        var result = JsonSerializer.Serialize(new { error = message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Écriture de la réponse.
        await context.Response.WriteAsync(result);
    }
}