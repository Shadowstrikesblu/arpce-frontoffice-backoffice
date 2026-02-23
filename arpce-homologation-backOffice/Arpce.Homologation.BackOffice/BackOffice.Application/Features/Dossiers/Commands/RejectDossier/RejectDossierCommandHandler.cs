using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Dossiers.Commands.RejectDossier;

public class RejectDossierCommandHandler : IRequestHandler<RejectDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public RejectDossierCommandHandler(IApplicationDbContext context, IAuditService auditService, INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(RejectDossierCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Demande) // Correction : Propriété unique
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null) throw new Exception("Dossier introuvable.");

        var codeStatutRejet = "RefusDossier";
        var statutRejet = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == codeStatutRejet, cancellationToken);
        if (statutRejet == null) throw new Exception("Statut rejet introuvable.");

        var motif = await _context.MotifsRejets.FirstOrDefaultAsync(m => m.Code == request.MotifRejet.Code, cancellationToken);
        if (motif == null)
        {
            motif = new MotifRejet { Id = Guid.NewGuid(), Code = request.MotifRejet.Code, Libelle = request.MotifRejet.Libelle, Remarques = request.MotifRejet.Remarques, DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            _context.MotifsRejets.Add(motif);
        }

        dossier.IdStatut = statutRejet.Id;

        // Correction : Application du motif sur la demande unique
        if (dossier.Demande != null)
        {
            dossier.Demande.IdMotifRejet = motif.Id;
        }

        _context.Commentaires.Add(new Commentaire { Id = Guid.NewGuid(), IdDossier = dossier.Id, DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), CommentaireTexte = $"Dossier REJETÉ. Motif : {motif.Libelle}", NomInstructeur = "Système" });

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Instruction Dossier", $"Dossier {dossier.Numero} rejeté.", "REJET", dossier.Id);

        return true;
    }
}