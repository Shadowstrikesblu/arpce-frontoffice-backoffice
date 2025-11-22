using System.Net;
using System.Text.Json;

namespace BackOffice.Api.Middleware;

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
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        object errorResponse;
        HttpStatusCode statusCode;

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized; 
                errorResponse = new { title = "Accès Non Autorisé", detail = exception.Message, status = (int)statusCode };
                break;

            case InvalidOperationException:
                statusCode = HttpStatusCode.UnprocessableEntity;
                errorResponse = new { title = "Opération Invalide", detail = exception.Message, status = (int)statusCode };
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError; 
                errorResponse = new
                {
                    title = "Erreur Interne du Serveur",
                    detail = "Une erreur inattendue est survenue. Veuillez contacter le support technique.",
                    status = (int)statusCode
                };
                _logger.LogError(exception, "Une exception non gérée a été interceptée.");
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}