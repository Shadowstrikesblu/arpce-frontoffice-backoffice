using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.BackgroundServices;

public class DossierAutomationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DossierAutomationWorker> _logger;

    public DossierAutomationWorker(IServiceProvider serviceProvider, ILogger<DossierAutomationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Moteur d'automatisation des délais DAFC et DRSCE démarré.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var devisGenerator = scope.ServiceProvider.GetRequiredService<IDevisGeneratorService>();

                    await ProcessPaiementsEnRetard(context, emailService, devisGenerator);
                    await ProcessEchantillonsEnRetard(context, emailService);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans le cycle d'automatisation des dossiers.");
            }

            // Vérification toutes les 6 heures
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }

    private async Task ProcessPaiementsEnRetard(IApplicationDbContext context, IEmailService email, IDevisGeneratorService devisGenerator)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Conversion des jours en millisecondes
        long deuxJoursMs = 2L * 24 * 60 * 60 * 1000;
        long quatreJoursMs = 4L * 24 * 60 * 60 * 1000;
        long trenteJoursMs = 30L * 24 * 60 * 60 * 1000;

        // On cherche les dossiers avec devis émis et non payés
        var dossiers = await context.Dossiers
            .Include(d => d.Client)
            .Include(d => d.Devis)
            .Where(d => d.DateEnvoiDevis != null && d.Devis.Any(dev => dev.PaiementOk != 1))
            .ToListAsync();

        foreach (var dossier in dossiers)
        {
            var devis = dossier.Devis.FirstOrDefault(dev => dev.PaiementOk != 1);
            if (devis == null) continue;

            long tempsEcoule = now - dossier.DateEnvoiDevis.Value;

            // RÈGLE 1 : J+30 -> Rejet définitif
            if (tempsEcoule >= trenteJoursMs)
            {
                var statutRefus = await context.Statuts.FirstOrDefaultAsync(s => s.Code == "RefusDossier");
                if (statutRefus != null && dossier.IdStatut != statutRefus.Id)
                {
                    dossier.IdStatut = statutRefus.Id;
                    await context.SaveChangesAsync(default);

                    await email.SendEmailAsync(dossier.Client.Email,
                        "[ALERTE] Dossier Rejeté - Délai de paiement expiré",
                        $"Le délai légal d'un mois pour le règlement du dossier {dossier.Numero} est dépassé. Votre demande d'homologation a été rejetée.");

                    _logger.LogWarning("Dossier {Num} rejeté pour non-paiement après 1 mois.", dossier.Numero);
                }
            }
            // RÈGLE 2 : J+4 -> Pénalité 10% + Facture de pénalité (SANS REJETER LE DOSSIER)
            else if (tempsEcoule >= quatreJoursMs && !dossier.PenaliteAppliquee)
            {
                decimal totalBase = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0);
                devis.MontantPenalite = totalBase * 0.10m;
                dossier.PenaliteAppliquee = true;

                // On sauvegarde d'abord la pénalité en base
                await context.SaveChangesAsync(default);

                // On regénère le PDF (le service lira le nouveau MontantPenalite en base et l'affichera)
                await devisGenerator.GenerateAndSaveDevisPdfAsync(devis.Id);

                // On informe le client
                await email.SendEmailAsync(dossier.Client.Email,
                    "[IMPORTANT] Pénalité de retard appliquée",
                    $"Le délai de 4 jours est expiré pour le dossier {dossier.Numero}. Une pénalité de 10% a été appliquée. Une nouvelle facture majorée est disponible sur votre espace.");

                _logger.LogInformation("Pénalité de 10% appliquée au dossier {Num}. Nouvelle facture générée.", dossier.Numero);
            }
            // RÈGLE 3 : J+2 -> Simple rappel
            else if (tempsEcoule >= deuxJoursMs && tempsEcoule < quatreJoursMs && !dossier.RappelPaiementEnvoye)
            {
                dossier.RappelPaiementEnvoye = true;
                await context.SaveChangesAsync(default);

                await email.SendEmailAsync(dossier.Client.Email,
                    "[RAPPEL] Échéance de paiement imminente",
                    $"Bonjour, sauf erreur de notre part, la facture pour le dossier {dossier.Numero} n'a pas encore été réglée. Attention, au-delà du 4ème jour, une pénalité de 10% sera appliquée.");
            }
        }
    }

    private async Task ProcessEchantillonsEnRetard(IApplicationDbContext context, IEmailService email)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long troisSemainesMs = 21L * 24 * 60 * 60 * 1000;
        long unMoisMs = 30L * 24 * 60 * 60 * 1000;

        var dossiers = await context.Dossiers
            .Include(d => d.Client)
            .Include(d => d.Demande)
            .Where(d => d.DateDemandeEchantillon != null && !d.Demande.EchantillonSoumis)
            .ToListAsync();

        foreach (var d in dossiers)
        {
            long tempsEcoule = now - d.DateDemandeEchantillon.Value;

            // REJET 1 MOIS (4 semaines)
            if (tempsEcoule >= unMoisMs)
            {
                var statutRefus = await context.Statuts.FirstOrDefaultAsync(s => s.Code == "RefusDossier");
                if (statutRefus != null && d.IdStatut != statutRefus.Id)
                {
                    d.IdStatut = statutRefus.Id;
                    await context.SaveChangesAsync(default);

                    await email.SendEmailAsync(d.Client.Email,
                        "Dossier Rejeté - Absence d'échantillon",
                        $"Le délai d'un mois est dépassé. Le dossier {d.Numero} est rejeté car l'échantillon n'a pas été déposé.");
                }
            }
            // RAPPEL 3 SEMAINES
            else if (tempsEcoule >= troisSemainesMs && tempsEcoule < unMoisMs && !d.RappelEchantillonEnvoye)
            {
                d.RappelEchantillonEnvoye = true;
                await context.SaveChangesAsync(default);

                await email.SendEmailAsync(d.Client.Email,
                    "Rappel : Dépôt échantillon requis",
                    $"Il vous reste une semaine pour déposer l'échantillon du dossier {d.Numero} avant rejet automatique.");
            }
        }
    }
}