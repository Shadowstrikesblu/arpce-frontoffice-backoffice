using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Dossiers.Commands.ValidateInstruction;

/// <summary>
/// Gère la logique de la commande pour valider l'instruction d'un dossier.
/// </summary>
public class ValidateInstructionCommandHandler : IRequestHandler<ValidateInstructionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ValidateInstructionCommandHandler> _logger;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public ValidateInstructionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ValidateInstructionCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Exécute la logique de validation d'instruction.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Levée si l'utilisateur n'est pas authentifié.</exception>
    /// <exception cref="Exception">Levée si le dossier est introuvable ou n'est pas dans un état valide pour cette action.</exception>
    public async Task<bool> Handle(ValidateInstructionCommand request, CancellationToken cancellationToken)
    {
        var agentId = _currentUserService.UserId;
        if (!agentId.HasValue)
        {
            throw new UnauthorizedAccessException("Accès non autorisé. L'authentification de l'agent est requise.");
        }

        // Récupére le dossier à valider.
        var dossier = await _context.Dossiers
            .Include(d => d.Statut) 
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");
        }

        // Vérifie si le dossier est dans un statut approprié pour être validé.
        if (dossier.Statut?.Code != StatutDossierEnum.EnCoursInstruction.ToString())
        {
            throw new InvalidOperationException($"L'opération de validation n'est pas autorisée pour un dossier avec le statut '{dossier.Statut?.Libelle}'. Le dossier doit être 'En cours d'instruction'.");
        }

        // Récupére le nouveau statut "Envoyé pour approbation".
        var nouveauStatut = await _context.Statuts
            .FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.EnvoyePourApprobation.ToString(), cancellationToken);

        if (nouveauStatut == null)
        {
            throw new Exception("Configuration système manquante : le statut 'Envoyé pour approbation' est introuvable.");
        }

        // Mettre à jour le statut du dossier.
        dossier.IdStatut = nouveauStatut.Id;

        _logger.LogInformation("Le statut du dossier {DossierId} a été changé à '{NouveauStatut}' par l'agent {AgentId}.",
            dossier.Id, nouveauStatut.Libelle, agentId.Value);

        // Ajoute un commentaire avec la remarque de validation.
        if (!string.IsNullOrWhiteSpace(request.Remarque))
        {
            var agent = await _context.AdminUtilisateurs.FindAsync(agentId.Value);
            var nomAgent = agent != null ? $"{agent.Prenoms} {agent.Nom}" : "Agent Inconnu";

            var commentaire = new Commentaire
            {
                IdDossier = dossier.Id,
                DateCommentaire = DateTime.UtcNow,
                CommentaireTexte = request.Remarque,
                NomInstructeur = nomAgent,
            };
            _context.Commentaires.Add(commentaire);
        }
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}