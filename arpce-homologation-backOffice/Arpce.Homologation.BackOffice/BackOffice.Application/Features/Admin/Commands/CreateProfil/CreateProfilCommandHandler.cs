using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.CreateProfil;

public class CreateProfilCommandHandler : IRequestHandler<CreateProfilCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateProfilCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateProfilCommand request, CancellationToken cancellationToken)
    {
        // Crée le profil
        var profil = new AdminProfils
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Libelle = request.Libelle,
            Remarques = request.Remarques,
            DateCreation = DateTime.UtcNow
        };

        _context.AdminProfils.Add(profil);

        // Crée les liaisons d'accès
        foreach (var accessDto in request.Access)
        {
            var profilAccess = new AdminProfilsAcces
            {
                IdProfil = profil.Id,
                IdAccess = accessDto.IdAccess,
                Ajouter = accessDto.Ajouter ? (byte)1 : (byte)0,
                Valider = accessDto.Valider ? (byte)1 : (byte)0,
                Supprimer = accessDto.Supprimer ? (byte)1 : (byte)0,
                Imprimer = accessDto.Imprimer ? (byte)1 : (byte)0
            };
            _context.AdminProfilsAcces.Add(profilAccess);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}