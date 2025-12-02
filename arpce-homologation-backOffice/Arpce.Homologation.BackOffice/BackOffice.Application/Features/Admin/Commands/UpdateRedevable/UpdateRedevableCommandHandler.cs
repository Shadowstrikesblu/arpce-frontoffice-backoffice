using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.UpdateRedevable;

public class UpdateRedevableCommandHandler : IRequestHandler<UpdateRedevableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateRedevableCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(UpdateRedevableCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(new object[] { request.Id }, cancellationToken);
        if (client == null) throw new Exception("Redevable introuvable.");

        // Mise à jour conditionnelle (PATCH)
        if (request.RaisonSociale != null) client.RaisonSociale = request.RaisonSociale;
        if (request.Email != null) client.Email = request.Email;
        if (request.ContactNom != null) client.ContactNom = request.ContactNom;
        if (request.ContactTelephone != null) client.ContactTelephone = request.ContactTelephone;
        if (request.TypeClient != null) client.TypeClient = request.TypeClient;
        if (request.Adresse != null) client.Adresse = request.Adresse;
        if (request.Ville != null) client.Ville = request.Ville;
        if (request.Pays != null) client.Pays = request.Pays;

        // Gestion du mot de passe
        if (!string.IsNullOrEmpty(request.MotPasse))
        {
            client.MotPasse = _passwordHasher.Hash(request.MotPasse);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}