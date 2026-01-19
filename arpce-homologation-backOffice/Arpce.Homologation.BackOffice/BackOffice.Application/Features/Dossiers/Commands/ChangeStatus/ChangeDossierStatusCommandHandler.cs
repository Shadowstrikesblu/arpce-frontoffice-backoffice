using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                    _logger.LogWarning("Impossible de générer PDF : aucun devis trouvé pour dossier {DossierId}", request.DossierId);
                }
            }

            if (request.CodeStatut == "DossierPayer")
            {
                await _certificateGenerator.GenerateAttestationsForDossierAsync(dossier.Id);
            }

            string targetGroup = "DOSSIERS";
            string notifType = "V";
            string title = "Changement de Statut";
            string message = $"Le statut du dossier {dossier.Numero} est maintenant : {nouveauStatut.Libelle}";
            bool shouldNotify = true;

            switch (request.CodeStatut)
            {
                case "Instruction": title = "Début Instruction"; message = $"Le dossier {dossier.Numero} est en cours d'instruction."; break;
                case "ApprobationInstruction": targetGroup = "DOSSIERS"; notifType = "T"; title = "Approbation Requise"; message = $"Le dossier {dossier.Numero} attend votre approbation."; break;
                case "InstructionApprouve": title = "Instruction Approuvée"; message = $"L'instruction pour {dossier.Numero} est approuvée."; break;
                case "Echantillon": title = "Échantillon Requis"; break;
                case "DevisCreer": title = "Devis Créé"; message = $"Le devis pour {dossier.Numero} est prêt."; break;
                case "DevisValideSC": targetGroup = "DEVIS"; notifType = "E"; title = "Devis Validé (Chef Service)"; break;
                case "DevisValideTr": targetGroup = "DEVIS"; notifType = "V"; title = "Devis Validé (Trésorerie)"; break;
                case "DevisEmit": title = "Devis Émis au Client"; break;
                case "PaiementRejete": title = "Paiement Rejeté"; break;
                case "DossierPayer": targetGroup = "PAIEMENTS"; notifType = "E"; title = "Paiement Confirmé"; message = $"Paiement confirmé pour {dossier.Numero}."; break;
                case "DossierSignature": targetGroup = "CERTIFICATS"; notifType = "V"; title = "Attestation en Signature"; break;
                case "DossierSigner": targetGroup = "CERTIFICATS"; notifType = "E"; title = "Attestation Signée"; break;
                default: shouldNotify = false; break;
            }

            if (shouldNotify)
            {
                await _notificationService.SendToGroupAsync(profilCode: targetGroup, title: title, message: message, type: notifType, targetUrl: $"/dossiers/{dossier.Id}", entityId: dossier.Id.ToString());
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