using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.SignalR;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<SignalRNotificationService> _logger;
    private readonly string _frontBaseUrl;

    public SignalRNotificationService(
        IHubContext<NotificationHub> hubContext,
        IApplicationDbContext context,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<SignalRNotificationService> logger)
    {
        _hubContext = hubContext;
        _context = context;
        _emailService = emailService;
        _logger = logger;
        _frontBaseUrl = configuration["AppSettings:FrontUrl"] ?? "https://app.arpce.cg";
    }

    /// <summary>
    /// Envoie une notification complète (SignalR + Emails au groupe) basée sur un événement métier.
    /// </summary>
    public async Task SendEventNotificationAsync(NotificationEvent eventType, Guid entityId)
    {
        var dossier = await _context.Dossiers
            .AsNoTracking()
            .Include(d => d.Demande)
            .FirstOrDefaultAsync(d => d.Id == entityId);

        if (dossier == null) return;

        var mapping = GetEventMapping(eventType, dossier);
        string fullUrl = $"{_frontBaseUrl}{mapping.route.Replace(":id", entityId.ToString())}";

        await SendToGroupAsync(mapping.profil, mapping.sujet, mapping.message, "Info", fullUrl, entityId.ToString());

        var agents = await _context.AdminUtilisateurs
            .AsNoTracking()
            .Where(u => u.Profil != null && u.Profil.Code == mapping.profil && !u.Desactive)
            .Select(u => new { u.Email, u.Compte, u.Nom, u.Prenoms })
            .ToListAsync();

        foreach (var agent in agents)
        {
            string targetEmail = !string.IsNullOrEmpty(agent.Email) ? agent.Email : agent.Compte;

            if (string.IsNullOrEmpty(targetEmail) || !targetEmail.Contains("@")) continue;

            try
            {
                string body = $@"
                    <div style='font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; color: #333;'>
                        <h2 style='color: #009A44;'>{mapping.sujet}</h2>
                        <p>Bonjour {agent.Prenoms} {agent.Nom},</p>
                        <p>{mapping.message}</p>
                        <div style='background-color: #f4f4f4; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <strong>Dossier N° :</strong> {dossier.Numero}<br/>
                            <strong>Libellé :</strong> {dossier.Libelle}<br/>
                            <strong>Équipement :</strong> {dossier.Demande?.Equipement ?? "N/A"}
                        </div>
                        <a href='{fullUrl}' style='display: inline-block; padding: 12px 25px; background-color: #009A44; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Accéder au dossier
                        </a>
                        <p style='font-size: 12px; color: #777; margin-top: 30px;'>
                            Ceci est une notification automatique générée par la plateforme d'homologation ARPCE.
                        </p>
                    </div>";

                await _emailService.SendEmailAsync(targetEmail, mapping.sujet, body);

                _context.Notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = mapping.sujet,
                    Message = mapping.message,
                    Type = "Email",
                    Canal = "EMAIL",
                    Destinataire = targetEmail,
                    ProfilCode = mapping.profil,
                    EntityId = entityId.ToString(),
                    TargetUrl = fullUrl,
                    DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    StatutEnvoi = "SUCCESS",
                    UtilisateurCreation = "SYSTEM_NOTIF"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec de l'envoi de l'email de notification à {Email}", targetEmail);
            }
        }

        await _context.SaveChangesAsync(default);
    }

    private (string profil, string route, string sujet, string message) GetEventMapping(NotificationEvent eventType, Dossier dossier)
    {
        return eventType switch
        {
            NotificationEvent.NouveauDossier =>
                ("DRSCE", "/platform/drsce/dossiers/:id", "Nouveau Dossier Déposé", $"Un nouveau dossier a été créé par un redevable."),

            NotificationEvent.DemandeApprobation =>
                ("DRSCE", "/platform/drsce/dossiers/:id", "Demande d'approbation", $"Une approbation technique est requise pour le dossier {dossier.Numero}."),

            NotificationEvent.NouveauDevisComptable =>
                ("DAFC", "/platform/dafc/devis_c/:id", "Nouveau Devis à traiter", $"Un devis a été généré et attend son traitement comptable."),

            NotificationEvent.DossierRefuse =>
                ("DRSCE", "/platform/drsce/all/:id", "Dossier Refusé", $"Le dossier {dossier.Numero} a fait l'objet d'un refus."),

            NotificationEvent.NouveauDevisChefService =>
                ("DAFC", "/platform/dafc/devis_t/:id", "Validation Devis (Chef de Service)", $"Le devis pour le dossier {dossier.Numero} doit être validé."),

            NotificationEvent.NouveauDevisDirecteur =>
                ("DAFC", "/platform/dafc/devis_d/:id", "Validation Devis (Directeur)", $"La signature du Directeur est attendue pour le devis {dossier.Numero}."),

            NotificationEvent.NouveauPaiement =>
                ("DAFC", "/platform/dafc/paiements/:id", "Notification de Paiement", $"Une preuve de paiement a été soumise pour le dossier {dossier.Numero}."),

            NotificationEvent.PaiementRefuse =>
                ("DAFC", "/platform/dafc/all/:id", "Paiement Rejeté", $"Le paiement concernant le dossier {dossier.Numero} a été refusé par la trésorerie."),

            NotificationEvent.NouveauProjetCertification =>
                ("DAJI", "/platform/daji/dossiers/:id", "Nouveau Projet de Certification", $"Un projet de certificat est en attente de validation juridique."),

            NotificationEvent.NouveauCertificat =>
                ("DRSCE", "/platform/drsce/certification/:id", "Nouveau Certificat", $"Le certificat d'homologation définitif a été généré pour {dossier.Numero}."),

            NotificationEvent.SignatureEnAttente =>
                ("DG", "/platform/dg/signature/:id", "Signature en attente (DG)", $"Le certificat {dossier.Numero} est en attente de la signature finale du Directeur Général."),

            _ => ("ADMIN", "/platform/admin/dossiers/:id", "Alerte Système", "Une action a été effectuée sur un dossier.")
        };
    }

    public async Task SendToUserAsync(string userId, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        var log = CreateSystemLog(title, message, type, targetUrl, entityId);
        log.UserId = Guid.Parse(userId);
        _context.Notifications.Add(log);
        await _context.SaveChangesAsync(default);

        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToGroupAsync(string groupName, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        var log = CreateSystemLog(title, message, type, targetUrl, entityId);
        log.ProfilCode = groupName;
        _context.Notifications.Add(log);
        await _context.SaveChangesAsync(default);

        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToAllAsync(string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        var log = CreateSystemLog(title, message, type, targetUrl, entityId);
        log.IsBroadcast = true;
        _context.Notifications.Add(log);
        await _context.SaveChangesAsync(default);

        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
    }

    private Notification CreateSystemLog(string title, string message, string type, string? url, string? entityId)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = url,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_NOTIF",
            Canal = "SYSTEM",
            StatutEnvoi = "SUCCESS"
        };
    }
}