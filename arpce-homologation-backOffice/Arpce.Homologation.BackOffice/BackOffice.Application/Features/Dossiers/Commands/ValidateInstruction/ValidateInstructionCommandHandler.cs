using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Dossiers.Commands.ValidateInstruction;

public class ValidateInstructionCommandHandler : IRequestHandler<ValidateInstructionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ValidateInstructionCommandHandler> _logger;
    private readonly IAuditService _auditService;

    public ValidateInstructionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ValidateInstructionCommandHandler> logger, IAuditService auditService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
        _auditService = auditService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ValidateInstructionCommand request, CancellationToken cancellationToken)
    {
        var agentId = _currentUserService.UserId;
        if (!agentId.HasValue)
        {
            throw new UnauthorizedAccessException("Accès non autorisé. L'authentification de l'agent est requise.");
        }

        var dossier = await _context.Dossiers
            .Include(d => d.Statut)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");
        }

        if (dossier.Statut?.Code != StatutDossierEnum.Instruction.ToString())
        {
            throw new InvalidOperationException($"L'opération de validation n'est pas autorisée. Le dossier est actuellement au statut '{dossier.Statut?.Libelle}' mais doit être 'En cours d'instruction' ({StatutDossierEnum.Instruction}).");
        }

        var nouveauStatut = await _context.Statuts
            .FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.ApprobationInstruction.ToString(), cancellationToken);

        if (nouveauStatut == null)
        {
            throw new Exception($"Configuration système manquante : le statut '{StatutDossierEnum.ApprobationInstruction}' est introuvable.");
        }

        dossier.IdStatut = nouveauStatut.Id;

        _logger.LogInformation("Le statut du dossier {DossierId} a été changé à '{NouveauStatut}' par l'agent {AgentId}.",
            dossier.Id, nouveauStatut.Libelle, agentId.Value);

        if (!string.IsNullOrWhiteSpace(request.Remarque))
        {
            var agent = await _context.AdminUtilisateurs.FindAsync(agentId.Value);
            var nomAgent = agent != null ? $"{agent.Prenoms} {agent.Nom}" : "Agent Inconnu";

            var commentaire = new Commentaire
            {
                Id = Guid.NewGuid(),
                IdDossier = dossier.Id,
                DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CommentaireTexte = request.Remarque,
                NomInstructeur = nomAgent
            };
            _context.Commentaires.Add(commentaire);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Validation Dossier",
            libelle: $"L'instruction du dossier '{dossier.Numero}' a été validée.",
            eventTypeCode: "VALIDATION",
            dossierId: dossier.Id);

        return true;
    }
}