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
    private readonly INotificationService _notificationService;
    // Injection du service de génération
    private readonly ICertificateGeneratorService _certificateGenerator;

    public ChangeDossierStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        INotificationService notificationService,
        ICertificateGeneratorService certificateGenerator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _notificationService = notificationService;
        _certificateGenerator = certificateGenerator;
    }

    public async Task<bool> Handle(ChangeDossierStatusCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers.FindAsync(new object[] { request.DossierId }, cancellationToken);
        if (dossier == null) throw new Exception("Dossier introuvable.");

        var nouveauStatut = await _context.Statuts
            .FirstOrDefaultAsync(s => s.Code == request.CodeStatut, cancellationToken);

        if (nouveauStatut == null) throw new Exception($"Statut '{request.CodeStatut}' introuvable.");

        // Mise à jour du statut
        dossier.IdStatut = nouveauStatut.Id;

        // Commentaire
        if (!string.IsNullOrWhiteSpace(request.Commentaire))
        {
            var agentId = _currentUserService.UserId;
            string nomAgent = "Système";
            if (agentId.HasValue)
            {
                var agent = await _context.AdminUtilisateurs.FindAsync(new object[] { agentId.Value }, cancellationToken);
                if (agent != null) nomAgent = $"{agent.Prenoms} {agent.Nom}";
            }

            _context.Commentaires.Add(new Commentaire
            {
                Id = Guid.NewGuid(),
                IdDossier = dossier.Id,
                DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CommentaireTexte = request.Commentaire,
                NomInstructeur = nomAgent
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("Gestion des Dossiers", $"Statut changé vers '{dossier.Libelle}'.", "MODIFICATION", dossier.Id);

        if (request.CodeStatut == "DossierPayer")
        {
            await _certificateGenerator.GenerateAttestationsForDossierAsync(dossier.Id);
        }
        string targetGroup = "DOSSIERS";
        string notifType = "V";
        string msg = $"Le statut du dossier {dossier.Numero} est maintenant : {nouveauStatut.Libelle}";

        switch (request.CodeStatut)
        {
            case "Echantillon": targetGroup = "DOSSIERS"; notifType = "V"; break;
            case "DevisCreer": targetGroup = "DOSSIERS"; notifType = "V"; break;
            case "DevisValideSC": targetGroup = "DEVIS"; notifType = "E"; break;
            case "DevisValideTr": targetGroup = "DEVIS"; notifType = "V"; break;
            case "DevisEmit": targetGroup = "DOSSIERS"; notifType = "V"; break;
            case "PaiementRejete": targetGroup = "DOSSIERS"; notifType = "V"; break;

            case "DossierPayer":
                targetGroup = "DOSSIERS";
                notifType = "V";
                msg = $"Paiement confirmé pour le dossier {dossier.Numero}. Certificats générés.";
                break;
        }

        await _notificationService.SendToGroupAsync(
            profilCode: targetGroup,
            title: "Changement de Statut",
            message: msg,
            type: notifType,
            targetUrl: $"/dossiers/{dossier.Id}",
            entityId: dossier.Id.ToString()
        );

        return true;
    }
}