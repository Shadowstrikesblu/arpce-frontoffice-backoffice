using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Commands.RejectDossier;

public class RejectDossierCommandHandler : IRequestHandler<RejectDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RejectDossierCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(RejectDossierCommand request, CancellationToken cancellationToken)
    {
        // Récupére le dossier
        var dossier = await _context.Dossiers
            .Include(d => d.Demandes) // Inclure les demandes pour leur assigner le motif
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null) throw new Exception("Dossier introuvable.");

        // Trouve le statut de rejet
        var statutRejet = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == request.Status, cancellationToken);
        if (statutRejet == null) throw new Exception($"Statut '{request.Status}' introuvable.");

        // Gére le Motif de Rejet
        // On vérifie si ce motif existe déjà, sinon on le crée (ou on le met à jour).
        // Ici, on va chercher par Code.
        var motif = await _context.MotifsRejets.FirstOrDefaultAsync(m => m.Code == request.MotifRejet.Code, cancellationToken);
        if (motif == null)
        {
            motif = new MotifRejet
            {
                Id = Guid.NewGuid(),
                Code = request.MotifRejet.Code,
                Libelle = request.MotifRejet.Libelle,
                Remarques = request.MotifRejet.Remarques,
                DateCreation = DateTime.UtcNow
            };
            _context.MotifsRejets.Add(motif);
            // Sauvegarde pour avoir l'ID si on vient de le créer
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Applique les changements
        dossier.IdStatut = statutRejet.Id;

        // Assigne le motif de rejet à toutes les demandes du dossier (si c'est un rejet global)
        foreach (var demande in dossier.Demandes)
        {
            demande.IdMotifRejet = motif.Id;
        }

        // Ajoute un commentaire explicatif
        var commentaire = new Commentaire
        {
            Id = Guid.NewGuid(),
            IdDossier = dossier.Id,
            DateCommentaire = DateTime.UtcNow,
            CommentaireTexte = $"Dossier REJETÉ. Motif : {motif.Libelle}. Remarques : {motif.Remarques}",
            NomInstructeur = "Système/Agent" // Idéalement, récupérer le nom de l'agent via ICurrentUserService
        };
        _context.Commentaires.Add(commentaire);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}