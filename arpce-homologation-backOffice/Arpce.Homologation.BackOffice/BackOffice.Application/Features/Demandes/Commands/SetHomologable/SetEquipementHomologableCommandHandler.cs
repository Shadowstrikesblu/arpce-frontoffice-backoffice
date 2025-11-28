// Fichier : BackOffice.Application/Features/Demandes/Commands/SetHomologable/SetEquipementHomologableCommandHandler.cs

using BackOffice.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Commands.SetHomologable;

public class SetEquipementHomologableCommandHandler : IRequestHandler<SetEquipementHomologableCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public SetEquipementHomologableCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SetEquipementHomologableCommand request, CancellationToken cancellationToken)
    {
        // Récupérer l'équipement
        var demande = await _context.Demandes.FindAsync(new object[] { request.EquipementId }, cancellationToken);

        if (demande == null)
        {
            throw new Exception($"L'équipement (demande) avec l'ID '{request.EquipementId}' est introuvable.");
        }

        // Mise à jour de la propriété
        demande.EstHomologable = request.Homologable;

        // Sauvegarde
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}