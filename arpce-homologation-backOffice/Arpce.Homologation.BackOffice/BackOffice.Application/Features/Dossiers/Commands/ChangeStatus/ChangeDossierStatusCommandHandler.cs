using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
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
    private readonly IReceiptGeneratorService _receiptGenerator;
    private readonly ILogger<ChangeDossierStatusCommandHandler> _logger;

    public ChangeDossierStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        INotificationService notificationService,
        ICertificateGeneratorService certificateGenerator,
        IDevisGeneratorService devisGeneratorService,
        IReceiptGeneratorService receiptGenerator,
        ILogger<ChangeDossierStatusCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _notificationService = notificationService;
        _certificateGenerator = certificateGenerator;
        _devisGeneratorService = devisGeneratorService;
        _receiptGenerator = receiptGenerator;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangeDossierStatusCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers
                .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
                .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

            if (dossier == null) throw new Exception("Dossier introuvable.");

            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == request.CodeStatut, cancellationToken);
            if (nouveauStatut == null) throw new Exception("Statut introuvable.");

            dossier.IdStatut = nouveauStatut.Id;
            dossier.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (!string.IsNullOrWhiteSpace(request.Commentaire))
            {
                var agent = await _context.AdminUtilisateurs.AsNoTracking().FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken);
                _context.Commentaires.Add(new Commentaire
                {
                    Id = Guid.NewGuid(),
                    IdDossier = dossier.Id,
                    DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    CommentaireTexte = request.Commentaire,
                    NomInstructeur = agent != null ? $"{agent.Prenoms} {agent.Nom}" : "Système"
                });
            }

            if (request.CodeStatut == "InstructionApprouve")
            {
                if (dossier.Demande != null && dossier.Demande.EstHomologable)
                {
                    await PerformFeeRecalculation(dossier, cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Dossier {Num} : Pas de calcul de devis (Équipement non homologable).", dossier.Numero);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            NotificationEvent? eventToTrigger = request.CodeStatut switch
            {
                "Instruction" => NotificationEvent.NouveauDossier,
                "ApprobationInstruction" => NotificationEvent.DemandeApprobation,
                "InstructionApprouve" => NotificationEvent.NouveauDevisComptable,
                "PaiementBanque" => NotificationEvent.NouveauPaiement,
                "DossierSignature" => NotificationEvent.NouveauProjetCertification,
                "DossierSigner" => NotificationEvent.NouveauCertificat,
                "RefusDossier" => NotificationEvent.DossierRefuse,
                _ => null
            };

            if (eventToTrigger.HasValue)
            {
                await _notificationService.SendEventNotificationAsync(eventToTrigger.Value, dossier.Id);
            }

            if (request.CodeStatut == "DevisEmit")
            {
                var devis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == request.DossierId, cancellationToken);
                if (devis != null) await _devisGeneratorService.GenerateAndSaveDevisPdfAsync(devis.Id);
            }

            if (request.CodeStatut == "PaiementCaisse")
            {
                var devis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == request.DossierId, cancellationToken);
                if (devis != null)
                {
                    decimal total = devis.MontantTotal; 
                    await _receiptGenerator.GenerateReceiptPdfAsync(dossier.Id, total, "Caisse", "Q-" + dossier.Numero);
                }
            }

            if (request.CodeStatut == "DossierSignature")
            {
                await _certificateGenerator.GenerateAttestationsForDossierAsync(dossier.Id);
            }

            await _auditService.LogAsync("Gestion des Dossiers", $"Passage au statut : {nouveauStatut.Libelle}", "MODIFICATION", dossier.Id);

            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erreur changement statut {DossierId}", request.DossierId);
            throw;
        }
    }

    private async Task PerformFeeRecalculation(Dossier dossier, CancellationToken ct)
    {
        var demande = dossier.Demande;
        if (demande?.CategorieEquipement == null) return;

        var cat = demande.CategorieEquipement;
        int qty = demande.QuantiteEquipements ?? 1;

        decimal etude = cat.FraisEtude ?? 0m;
        decimal controle = cat.FraisControle ?? 0m;
        decimal homologation = 0m;

        if (cat.ModeCalcul == ModeCalcul.PER_BLOCK)
        {
            decimal blocks = Math.Ceiling((decimal)qty / (cat.BlockSize ?? 50));
            homologation = (cat.FraisHomologation ?? 0m) * blocks;
        }
        else if (cat.ModeCalcul == ModeCalcul.PER_UNIT)
            homologation = (cat.FraisHomologation ?? 0m) * qty;
        else
            homologation = cat.FraisHomologation ?? 0m;

        decimal totalBase = etude + homologation + controle;
        demande.PrixUnitaire = totalBase;

        var devis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == dossier.Id, ct);
        if (devis == null)
        {
            devis = new Devis { Id = Guid.NewGuid(), IdDossier = dossier.Id, IdDemande = demande.Id };
            _context.Devis.Add(devis);
        }

        devis.Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        devis.MontantEtude = etude;
        devis.MontantHomologation = homologation;
        devis.MontantControle = controle;

        devis.MontantTotal = totalBase + devis.MontantPenalite;
    }
}