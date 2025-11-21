using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Queries.ConnectByToken;

/// <summary>
/// Gère la logique de la requête de validation de session par token.
/// </summary>
public class ConnectByTokenQueryHandler : IRequestHandler<ConnectByTokenQuery, ConnectByTokenResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public ConnectByTokenQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la logique de validation de session.
    /// </summary>
    public async Task<ConnectByTokenResult> Handle(ConnectByTokenQuery request, CancellationToken cancellationToken)
    {
        // Récupére l'ID de l'utilisateur depuis le token (déjà validé par le middleware).
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            // Cette situation ne devrait jamais se produire si l'endpoint est bien protégé par [Authorize],
            // mais c'est une sécurité supplémentaire.
            throw new UnauthorizedAccessException("Token invalide ou session expirée.");
        }

        // Récupére les informations de l'utilisateur depuis la base de données.
        var client = await _context.Clients
            .AsNoTracking() // Optimisation pour une requête en lecture seule.
            .FirstOrDefaultAsync(c => c.Id == userId.Value, cancellationToken);

        if (client == null)
        {
            // L'utilisateur n'existe plus dans la base de données, même si le token est valide.
            throw new UnauthorizedAccessException("L'utilisateur associé à ce token n'existe plus.");
        }

        // Retourne les informations de l'utilisateur.
        return new ConnectByTokenResult
        {
            Message = "Session validée avec succès.",
            UserId = client.Id,
            Email = client.Email ?? string.Empty,
            RaisonSociale = client.RaisonSociale,
            ContactNom = client.ContactNom
        };
    }
}