using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Commands.RejectDossier;

public class RejectDossierCommandHandler : IRequestHandler<RejectDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService; 

    public RejectDossierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(RejectDossierCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Demandes)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null) throw new Exception("Dossier introuvable.");

        // Statut de rejet (RefusDossier selon PDF)
        var codeStatutRejet = "RefusDossier"; 
        var statutRejet = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == codeStatutRejet, cancellationToken);

        if (statutRejet == null)
            statutRejet = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == request.Status, cancellationToken);

        if (statutRejet == null) throw new Exception($"Statut '{codeStatutRejet}' introuvable.");

        // Motif
        var motif = await _context.MotifsRejets.FirstOrDefaultAsync(m => m.Code == request.MotifRejet.Code, cancellationToken);
        if (motif == null)
        {
            motif = new MotifRejet
            {
                Id = Guid.NewGuid(),
                Code = request.MotifRejet.Code,
                Libelle = request.MotifRejet.Libelle,
                Remarques = request.MotifRejet.Remarques,
                DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            };
            _context.MotifsRejets.Add(motif);
            await _context.SaveChangesAsync(cancellationToken);
        }

        dossier.IdStatut = statutRejet.Id;

        foreach (var demande in dossier.Demandes)
        {
            demande.IdMotifRejet = motif.Id;
        }

        var commentaire = new Commentaire
        {
            Id = Guid.NewGuid(),
            IdDossier = dossier.Id,
            DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            CommentaireTexte = $"Dossier REJETÉ. Motif : {motif.Libelle}. Remarques : {motif.Remarques}",
            NomInstructeur = "Système/Agent"
        };
        _context.Commentaires.Add(commentaire);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("Instruction Dossier", $"Dossier {dossier.Numero} rejeté.", "REJET", dossier.Id);

        await _notificationService.SendToGroupAsync(
            profilCode: "DOSSIERS",
            title: "Dossier Rejeté",
            message: $"Le dossier {dossier.Numero} a été refusé. Motif : {motif.Libelle}",
            type: "E",
            targetUrl: $"/dossiers/{dossier.Id}",
            entityId: dossier.Id.ToString()
        );

        return true;
    }
}