using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore; 

namespace BackOffice.Application.Features.Dossiers.Commands.ChangeStatus;

public class ChangeDossierStatusCommandHandler : IRequestHandler<ChangeDossierStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public ChangeDossierStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IAuditService auditService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ChangeDossierStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. Récupérer le dossier
        var dossier = await _context.Dossiers.FindAsync(new object[] { request.DossierId }, cancellationToken);
        if (dossier == null)
        {
            throw new Exception($"Dossier avec l'ID '{request.DossierId}' introuvable.");
        }

        // 2. Récupérer le nouveau statut par son Code
        var nouveauStatut = await _context.Statuts
            .FirstOrDefaultAsync(s => s.Code == request.CodeStatut, cancellationToken);

        if (nouveauStatut == null)
        {
            throw new Exception($"Le statut avec le code '{request.CodeStatut}' est introuvable.");
        }

        // 3. Appliquer le changement
        dossier.IdStatut = nouveauStatut.Id;

        // 4. Ajouter un commentaire (si fourni)
        if (!string.IsNullOrWhiteSpace(request.Commentaire))
        {
            var agentId = _currentUserService.UserId;
            string nomAgent = "Système";

            if (agentId.HasValue)
            {
                var agent = await _context.AdminUtilisateurs.FindAsync(new object[] { agentId.Value }, cancellationToken);
                if (agent != null) nomAgent = $"{agent.Prenoms} {agent.Nom}";
            }

            var commentaire = new Commentaire
            {
                Id = Guid.NewGuid(),
                IdDossier = dossier.Id,
                DateCommentaire = DateTime.UtcNow,
                CommentaireTexte = request.Commentaire,
                NomInstructeur = nomAgent
            };
            _context.Commentaires.Add(commentaire);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion des Dossiers",
    libelle: $"Le statut du dossier '{dossier.Numero}' a été changé à '{dossier.Libelle}'.",
    eventTypeCode: "MODIFICATION",
    dossierId: dossier.Id);

        return true;
    }
}