using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Demandes.Commands.CreateDossier;

public class CreateDossierCommandHandler : IRequestHandler<CreateDossierCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateDossierCommandHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public CreateDossierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<CreateDossierCommandHandler> logger,
        INotificationService notificationService,
        IEmailService emailService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateDossierCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Génération du Numéro officiel
                var currentYear = DateTime.UtcNow.ToString("yy");
                var prefix = $"Hom-{currentYear}-";
                var countThisYear = await _context.Dossiers.AsNoTracking()
                    .CountAsync(d => d.Numero.StartsWith(prefix), cancellationToken);
                var generatedNumero = $"{prefix}{(countThisYear + 1).ToString("D4")}";

                // Statut initial
                var defaultStatut = await _context.Statuts.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.NouveauDossier.ToString(), cancellationToken);

                // Création du Dossier
                var dossier = new Dossier
                {
                    Id = Guid.NewGuid(),
                    IdClient = userId.Value,
                    IdStatut = defaultStatut!.Id,
                    DateOuverture = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Numero = generatedNumero,
                    Libelle = request.Libelle,
                    DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    UtilisateurCreation = userId.Value.ToString()
                };

                // Création de la Demande
                var demande = new Demande
                {
                    Id = Guid.NewGuid(),
                    IdDossier = dossier.Id,
                    Equipement = request.Demande.Equipement,
                    Modele = request.Demande.Modele,
                    Marque = request.Demande.Marque,
                    Fabricant = request.Demande.Fabricant,
                    Type = request.Demande.Type,
                    Description = request.Demande.Description,
                    QuantiteEquipements = request.Demande.QuantiteEquipements,

                    PrixUnitaire = request.Demande.PrixUnitaire ?? 0,
                    EstHomologable = request.EstHomologable, 

                    RequiertEchantillon = request.Demande.RequiertEchantillon,
                    EchantillonSoumis = false,
                    DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    UtilisateurCreation = userId.Value.ToString()
                };

                // Création du Bénéficiaire
                Beneficiaire? beneficiaire = null;
                if (request.Demande.Beneficiaire != null)
                {
                    beneficiaire = new Beneficiaire
                    {
                        Id = Guid.NewGuid(),
                        DemandeId = demande.Id,
                        Nom = request.Demande.Beneficiaire.Nom,
                        Email = request.Demande.Beneficiaire.Email,
                        Telephone = request.Demande.Beneficiaire.Telephone,
                        Type = request.Demande.Beneficiaire.Type,
                        Adresse = request.Demande.Beneficiaire.Adresse,
                        DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        UtilisateurCreation = userId.Value.ToString()
                    };
                    _context.Beneficiaires.Add(beneficiaire);
                }

                _context.Dossiers.Add(dossier);
                _context.Demandes.Add(demande);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // --- LOGIQUE D'ENVOI DE MAIL AU BÉNÉFICIAIRE ---
                if (beneficiaire != null && !string.IsNullOrEmpty(beneficiaire.Email))
                {
                    await SendEmailToBeneficiary(beneficiaire, dossier, demande);
                }

                // Notification SignalR pour le Back Office
                await _notificationService.SendToGroupAsync(
                    groupName: "DRSCE",
                    title: "Nouveau Dossier Créé",
                    message: $"Le dossier {dossier.Numero} a été initié par le client.",
                    type: "V",
                    targetUrl: $"/platform/drsce/dossiers/{dossier.Id}"
                );

                return dossier.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Échec de la création du dossier {Libelle}", request.Libelle);
                throw;
            }
        });
    }

    private async Task SendEmailToBeneficiary(Beneficiaire beneficiaire, Dossier dossier, Demande demande)
    {
        try
        {
            var subject = $"[ARPCE] Ouverture de dossier d'homologation - {dossier.Numero}";
            var body = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h3>Bonjour {beneficiaire.Nom},</h3>
                    <p>Nous vous informons qu'un dossier d'homologation a été ouvert auprès de l'ARPCE pour l'équipement suivant :</p>
                    <ul>
                        <li><strong>Équipement :</strong> {demande.Equipement}</li>
                        <li><strong>Modèle :</strong> {demande.Modele}</li>
                        <li><strong>N° de Dossier :</strong> {dossier.Numero}</li>
                    </ul>
                    <p>Vous recevrez des notifications concernant l'évolution de ce dossier.</p>
                    <br/>
                    <p>Cordialement,<br/>L'équipe ARPCE</p>
                </div>";

            await _emailService.SendEmailAsync(beneficiaire.Email!, subject, body);

            var log = new Notification
            {
                Id = Guid.NewGuid(),
                Title = subject,
                Message = $"Email envoyé au bénéficiaire {beneficiaire.Nom}",
                Type = "Info",
                Canal = "EMAIL",
                Destinataire = beneficiaire.Email!,
                DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                StatutEnvoi = "SUCCESS"
            };
            _context.Notifications.Add(log);
            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi du mail au bénéficiaire {Email}", beneficiaire.Email);
        }
    }
}