using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Dossiers.Commands.ChangeStatus;

public class ChangeDossierStatusCommandHandler : IRequestHandler<ChangeDossierStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly ICertificateGeneratorService _certificateGenerator;
    private readonly IDevisGeneratorService _devisGeneratorService;
    private readonly ILogger<ChangeDossierStatusCommandHandler> _logger;

    public ChangeDossierStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        INotificationService notificationService,
        ICertificateGeneratorService certificateGenerator,
        IDevisGeneratorService devisGeneratorService,
        ILogger<ChangeDossierStatusCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _notificationService = notificationService;
        _certificateGenerator = certificateGenerator;
        _devisGeneratorService = devisGeneratorService;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangeDossierStatusCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers.FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);
            if (dossier == null) throw new Exception("Dossier introuvable.");

            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == request.CodeStatut, cancellationToken);
            if (nouveauStatut == null) throw new Exception($"Statut '{request.CodeStatut}' introuvable.");

            dossier.IdStatut = nouveauStatut.Id;
            dossier.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (!string.IsNullOrWhiteSpace(request.Commentaire))
            {
                var agentId = _currentUserService.UserId;
                string nomAgent = "Système";
                if (agentId.HasValue)
                {
                    var agent = await _context.AdminUtilisateurs.FindAsync(new object[] { agentId.Value }, cancellationToken);
                    if (agent != null) nomAgent = $"{agent.Prenoms} {agent.Nom}";
                }
                _context.Commentaires.Add(new Commentaire { Id = Guid.NewGuid(), IdDossier = dossier.Id, DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), CommentaireTexte = request.Commentaire, NomInstructeur = nomAgent });
            }

            await _context.SaveChangesAsync(cancellationToken);

            if (request.CodeStatut == "DevisEmit")
            {
                var devis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == request.DossierId, cancellationToken);
                if (devis != null)
                {
                    await _devisGeneratorService.GenerateAndSaveDevisPdfAsync(devis.Id);
                }
                else
                {
                    _logger.LogWarning("Impossible de générer le PDF : aucun devis trouvé pour le dossier {DossierId}", request.DossierId);
                }
            }

            if (request.CodeStatut == "DossierPayer")
            {
                await _certificateGenerator.GenerateAttestationsForDossierAsync(dossier.Id);
            }

            string targetGroup = "DRSCE"; 
            string notifType = "V";
            string title = "Changement de Statut";
            string message = $"Le statut du dossier {dossier.Numero} est maintenant : {nouveauStatut.Libelle}";
            bool shouldNotify = true;
            string? secondaryTargetGroup = null;
            string? secondaryNotifType = null;

            switch (request.CodeStatut)
            {
                case "Instruction": targetGroup = "DRSCE"; notifType = "E"; title = "Début Instruction"; message = $"Le dossier {dossier.Numero} est en cours d'instruction."; break;
                case "ApprobationInstruction": targetGroup = "DRSCE"; notifType = "T"; title = "Approbation Requise"; message = $"Le dossier {dossier.Numero} attend votre approbation."; break;
                case "InstructionApprouve": targetGroup = "DAFC"; notifType = "V"; title = "Instruction Approuvée"; message = $"L'instruction pour {dossier.Numero} est approuvée. Prêt pour la facturation."; secondaryTargetGroup = "DRSCE"; secondaryNotifType = "V"; break;
                case "Echantillon": targetGroup = "DRSCE"; notifType = "V"; title = "Échantillon Requis"; break;
                case "DevisCreer": targetGroup = "DAFC"; notifType = "V"; title = "Devis Créé"; message = $"Le devis pour le dossier {dossier.Numero} est prêt à être validé."; break;
                case "DevisValideSC": targetGroup = "DAFC"; notifType = "E"; title = "Devis Validé (CS)"; break;
                case "DevisValideTr": targetGroup = "DAFC"; notifType = "V"; title = "Devis Validé (Trésorerie)"; break;
                case "DevisEmit": targetGroup = "DAFC"; notifType = "V"; title = "Devis Émis au Client"; break;
                case "PaiementRejete": targetGroup = "DAFC"; notifType = "V"; title = "Paiement Rejeté"; break;
                case "PaiementBanque": targetGroup = "DAFC"; notifType = "E"; title = "Paiement par Banque"; message = $"Une preuve de paiement a été soumise pour le dossier {dossier.Numero}."; break;
                case "DossierPayer": targetGroup = "DRSCE"; notifType = "E"; title = "Paiement Confirmé"; message = $"Paiement confirmé pour {dossier.Numero}."; secondaryTargetGroup = "DAFC"; secondaryNotifType = "V"; break;
                case "DossierSignature": targetGroup = "DAJI"; notifType = "V"; title = "Attestation en Signature"; break;
                case "DossierSigner": targetGroup = "DRSCE"; notifType = "E"; title = "Attestation Signée"; secondaryTargetGroup = "DAJI"; secondaryNotifType = "V"; break;
                default: shouldNotify = false; break;
            }

            if (shouldNotify)
            {
                await _notificationService.SendToGroupAsync(profilCode: targetGroup, title: title, message: message, type: notifType, targetUrl: $"/dossiers/{dossier.Id}", entityId: dossier.Id.ToString());
                if (secondaryTargetGroup != null)
                {
                    await _notificationService.SendToGroupAsync(profilCode: secondaryTargetGroup, title: title, message: message, type: secondaryNotifType, targetUrl: $"/dossiers/{dossier.Id}", entityId: dossier.Id.ToString());
                }
            }

            await _auditService.LogAsync("Gestion des Dossiers", $"Statut du dossier '{dossier.Numero}' changé vers '{nouveauStatut.Libelle}'.", "MODIFICATION", dossier.Id);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec du changement de statut pour le dossier {DossierId}", request.DossierId);
            throw;
        }
    }
}