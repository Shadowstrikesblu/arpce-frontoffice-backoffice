using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Common.Exceptions; 
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Queries.ConnectByToken;

public class ConnectByTokenQueryHandler : IRequestHandler<ConnectByTokenQuery, ConnectByTokenResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConnectByTokenQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ConnectByTokenResult> Handle(ConnectByTokenQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Token invalide ou session expirée.");
        }

        var client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == userId.Value, cancellationToken);

        if (client == null)
        {
            throw new UnauthorizedAccessException("L'utilisateur associé à ce token n'existe plus.");
        }

        if (client.NiveauValidation < 2)
        {
            throw new AccountPendingValidationException("Votre compte est en attente de validation par l'administration (Niveau 2 requis).");
        }

        if (client.Desactive == 1)
        {
            throw new UnauthorizedAccessException("Ce compte a été désactivé.");
        }

        return new ConnectByTokenResult
        {
            Message = "Session validée avec succès.",
            UserId = client.Id,
            Email = client.Email ?? string.Empty,
            RaisonSociale = client.RaisonSociale,
            CodeClient = client.Code,
            RegistreCommerce = client.RegistreCommerce,
            ContactNom = client.ContactNom,
            ContactTelephone = client.ContactTelephone,
            ContactFonction = client.ContactFonction,
            Adresse = client.Adresse,
            Bp = client.Bp,
            Ville = client.Ville,
            Pays = client.Pays,
            IsVerified = client.IsVerified,
            NiveauValidation = client.NiveauValidation
        };
    }
}