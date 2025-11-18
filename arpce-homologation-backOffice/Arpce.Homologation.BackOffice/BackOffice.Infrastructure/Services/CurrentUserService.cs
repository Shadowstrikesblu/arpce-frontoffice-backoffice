using BackOffice.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BackOffice.Infrastructure.Services;

/// <summary>
/// Implémentation du service ICurrentUserService.
/// Extrait l'ID de l'utilisateur à partir des claims du token JWT présent dans la requête HTTP.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initialise une nouvelle instance du service.
    /// </summary>
    /// <param name="httpContextAccessor">
    /// Service ASP.NET Core qui donne accès au contexte de la requête HTTP actuelle.
    /// Il doit être enregistré dans le conteneur de dépendances.
    /// </param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Récupère l'ID de l'utilisateur depuis le claim 'NameIdentifier' (ou 'sub') du token JWT.
    /// </summary>
    public Guid? UserId
    {
        get
        {
            // On cherche le claim qui contient l'ID de l'utilisateur.
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}