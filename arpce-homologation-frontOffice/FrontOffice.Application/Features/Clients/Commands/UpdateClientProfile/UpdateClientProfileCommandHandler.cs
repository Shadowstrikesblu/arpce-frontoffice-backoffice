using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;

namespace FrontOffice.Application.Features.Clients.Commands.UpdateClientProfile;

public class UpdateClientProfileCommandHandler : IRequestHandler<UpdateClientProfileCommand, ClientProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateClientProfileCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ClientProfileDto> Handle(UpdateClientProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var client = await _context.Clients.FindAsync(new object[] { userId.Value }, cancellationToken);
        if (client == null) throw new UnauthorizedAccessException("Utilisateur introuvable.");

        if (request.RaisonSociale != null) client.RaisonSociale = request.RaisonSociale;
        if (request.RegistreCommerce != null) client.RegistreCommerce = request.RegistreCommerce;
        if (request.ContactNom != null) client.ContactNom = request.ContactNom;
        if (request.ContactTelephone != null) client.ContactTelephone = request.ContactTelephone;
        if (request.ContactFonction != null) client.ContactFonction = request.ContactFonction;
        if (request.Adresse != null) client.Adresse = request.Adresse;
        if (request.Bp != null) client.Bp = request.Bp;
        if (request.Ville != null) client.Ville = request.Ville;
        if (request.Pays != null) client.Pays = request.Pays;

        await _context.SaveChangesAsync(cancellationToken);

        return new ClientProfileDto
        {
            RaisonSociale = client.RaisonSociale,
            RegistreCommerce = client.RegistreCommerce,
            ContactNom = client.ContactNom,
            ContactTelephone = client.ContactTelephone,
            ContactFonction = client.ContactFonction,
            Adresse = client.Adresse,
            Bp = client.Bp,
            Ville = client.Ville,
            Pays = client.Pays
        };
    }
}