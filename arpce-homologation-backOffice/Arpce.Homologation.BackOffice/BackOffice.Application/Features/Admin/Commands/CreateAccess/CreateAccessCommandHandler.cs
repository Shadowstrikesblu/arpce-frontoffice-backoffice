using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.CreateAccess;

public class CreateAccessCommandHandler : IRequestHandler<CreateAccessCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateAccessCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateAccessCommand request, CancellationToken cancellationToken)
    {
        var access = new AdminAccess
        {
            Id = Guid.NewGuid(),
            Application = request.Application,
            Groupe = request.Groupe,
            Libelle = request.Libelle,
            Page = request.Page,
            Type = request.Type,
            Inactif = request.Inactif ? (byte)1 : (byte)0,
            Ajouter = request.Ajouter ? (byte)1 : (byte)0,
            Valider = request.Valider ? (byte)1 : (byte)0,
            Supprimer = request.Supprimer ? (byte)1 : (byte)0,
            Imprimer = request.Imprimer ? (byte)1 : (byte)0
        };

        _context.AdminAccesses.Add(access);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}