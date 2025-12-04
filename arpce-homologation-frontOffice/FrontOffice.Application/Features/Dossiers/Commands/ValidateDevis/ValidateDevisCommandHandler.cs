using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Commands.ValidateDevis;

/// <summary>
/// Gère la logique de la commande pour qu'un client valide un devis.
/// </summary>
public class ValidateDevisCommandHandler : IRequestHandler<ValidateDevisCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ValidateDevisCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(ValidateDevisCommand request, CancellationToken cancellationToken)
    {
        // Validation de l'utilisateur
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Récupére le dossier et vérifier les droits
        var dossier = await _context.Dossiers
            .Include(d => d.Statut) // Inclure le statut pour vérifier l'état
            .FirstOrDefaultAsync(d => d.Id == request.DossierId && d.IdClient == userId.Value, cancellationToken);

        if (dossier == null)
        {
            throw new Exception($"Dossier avec l'ID '{request.DossierId}' introuvable ou non autorisé.");
        }

        // Règle métier : vérifie que le dossier est bien en attente de validation de devis
        if (dossier.Statut?.Code != StatutDossierEnum.DevisEmit.ToString())
        {
            throw new InvalidOperationException($"Le devis de ce dossier n'est pas en attente de validation. Statut actuel : '{dossier.Statut?.Libelle}'.");
        }

        // Récupére le nouveau statut "Devis validé par client"
        var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.DevisValide.ToString(), cancellationToken);
        if (nouveauStatut == null)
        {
            throw new Exception("Configuration système manquante : le statut 'DevisValide' est introuvable.");
        }

        // Met à jour le statut du dossier
        dossier.IdStatut = nouveauStatut.Id;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}