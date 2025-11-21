using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Demandes.Commands.AddCategorieToDemande;

/// <summary>
/// Gère la logique de la commande pour assigner une catégorie à une demande.
/// </summary>
public class AddCategorieToDemandeCommandHandler : IRequestHandler<AddCategorieToDemandeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AddCategorieToDemandeCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService; 

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public AddCategorieToDemandeCommandHandler(
        IApplicationDbContext context,
        ILogger<AddCategorieToDemandeCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la logique d'assignation de la catégorie.
    /// </summary>
    /// <exception cref="Exception">Levée si la demande ou la catégorie est introuvable.</exception>
    public async Task<bool> Handle(AddCategorieToDemandeCommand request, CancellationToken cancellationToken)
    {
        // Récupére l'entité Demande (équipement) à mettre à jour.
        var demande = await _context.Demandes.FindAsync(request.DemandeId);

        if (demande == null)
        {
            _logger.LogWarning("Tentative d'assignation de catégorie à une demande inexistante. DemandeId: {DemandeId}", request.DemandeId);
            throw new Exception($"La demande (équipement) avec l'ID '{request.DemandeId}' est introuvable.");
        }

        // Vérifie que la catégorie à assigner existe bien.
        var categorieExists = await _context.CategoriesEquipements.AnyAsync(c => c.Id == request.CategorieId, cancellationToken);

        if (!categorieExists)
        {
            _logger.LogWarning("Tentative d'assignation d'une catégorie inexistante. CategorieId: {CategorieId}", request.CategorieId);
            throw new Exception($"La catégorie avec l'ID '{request.CategorieId}' est introuvable.");
        }

        // Effectue la mise à jour.
        demande.IdCategorie = request.CategorieId;

        _logger.LogInformation("La catégorie {CategorieId} a été assignée à la demande {DemandeId} par l'utilisateur {UserId}.",
            request.CategorieId, request.DemandeId, _currentUserService.UserId);

        // Sauvegarde les changements.
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}