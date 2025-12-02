using FrontOffice.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontOffice.Api.Middleware;

/// <summary>
/// Middleware pour intercepter de manière centralisée toutes les exceptions non gérées
/// et les transformer en une réponse HTTP JSON standardisée et compréhensible.
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

    /// <summary>
    /// Méthode principale du middleware, appelée pour chaque requête HTTP.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Tente d'exécuter le reste du pipeline de la requête.
            await _next(context);
        }
        catch (Exception ex)
        {
            // Si une exception se produit n'importe où dans le pipeline, elle est interceptée ici.
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Gère une exception interceptée en générant une réponse HTTP appropriée.
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Objet qui sera sérialisé en JSON pour la réponse.
        object errorResponse;
        // Code de statut HTTP à retourner.
        HttpStatusCode statusCode;

        // On utilise un 'switch' sur le type de l'exception pour adapter la réponse.
        switch (exception)
        {
            // --- Cas des erreurs d'autorisation ---
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized; 
                errorResponse = new
                {
                    title = "Accès Non Autorisé, vérifier vos infos de connexion",
                    detail = exception.Message, 
                    status = (int)HttpStatusCode.Unauthorized
                };
                break;

            // --- Cas spécifique : Compte en attente de validation ARPCE ---
            case AccountPendingValidationException:
                statusCode = HttpStatusCode.NotAcceptable; // Code 406
                errorResponse = new
                {
                    title = "Compte Non Validé",
                    detail = exception.Message,
                    status = (int)HttpStatusCode.NotAcceptable
                };
                break;

            // --- Cas des erreurs de validation ou métier prévues ---
            case InvalidOperationException:
                statusCode = HttpStatusCode.UnprocessableEntity; 
                errorResponse = new
                {
                    title = "Opération Invalide",
                    detail = exception.Message,
                    status = (int)HttpStatusCode.UnprocessableEntity
                };
                break;

            // --- Cas par défaut pour toutes les autres erreurs inattendues ---
            default:
                statusCode = HttpStatusCode.InternalServerError; 
                errorResponse = new
                {
                    title = "Erreur Interne du Serveur",
                    detail = "Une erreur inattendue est survenue. Veuillez contacter le support technique de CDS.",
                    status = (int)HttpStatusCode.InternalServerError
                };

                // On logue l'exception complète côté serveur pour le débogage,
                _logger.LogError(exception, "Une exception non gérée a été interceptée par le middleware.");
                break;
        }

        // Configuration de la réponse HTTP
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Écriture de la réponse JSON
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}