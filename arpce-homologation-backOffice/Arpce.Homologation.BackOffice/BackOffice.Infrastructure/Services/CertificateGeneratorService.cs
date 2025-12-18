using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BackOffice.Infrastructure.Services;

public class CertificateGeneratorService : ICertificateGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<CertificateGeneratorService> _logger;
    private readonly CertificateSettings _settings;

    // Chemin vers le logo
    private readonly string _logoPath;

    public CertificateGeneratorService(
        IApplicationDbContext context,
        IAuditService auditService,
        ILogger<CertificateGeneratorService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _auditService = auditService;
        _logger = logger;
        _settings = configuration.GetSection("CertificateSettings").Get<CertificateSettings>()
                    ?? new CertificateSettings();
        QuestPDF.Settings.License = LicenseType.Community;

        // Construction du chemin absolu vers le logo
        _logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo_arpce.png");
    }

    public async Task GenerateAttestationsForDossierAsync(Guid dossierId)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Demandes)
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == dossierId);

        if (dossier == null || !dossier.Demandes.Any()) return;

        int currentYear = DateTime.UtcNow.Year;

        // Calcul du prochain numéro de séquence global
        int maxSequence = await _context.Attestations
            .Where(a => a.DateDelivrance >= new DateTimeOffset(new DateTime(currentYear, 1, 1)).ToUnixTimeMilliseconds())
            .MaxAsync(a => (int?)a.NumeroSequentiel) ?? 0;

        int nextSequence = maxSequence + 1;

        // Chargement du logo en mémoire 
        byte[] logoBytes = Array.Empty<byte>();
        if (File.Exists(_logoPath))
        {
            logoBytes = await File.ReadAllBytesAsync(_logoPath);
        }
        else
        {
            _logger.LogWarning("Logo introuvable au chemin : {Path}", _logoPath);
        }

        foreach (var demande in dossier.Demandes)
        {
            var existingAttestation = await _context.Attestations.FirstOrDefaultAsync(a => a.IdDemande == demande.Id);

            int seqNumber;
            if (existingAttestation != null)
            {
                seqNumber = existingAttestation.NumeroSequentiel;
            }
            else
            {
                seqNumber = nextSequence;
                nextSequence++;
            }

            string formattedSeq = seqNumber.ToString("D4");
            string referenceNumber = $"N°/{formattedSeq}/ARPCE-DG/DAJI/DRSCE/{currentYear}";

            byte[] pdfBytes;

            if (demande.EstHomologable)
            {
                pdfBytes = GenerateCertificatePdf(dossier, demande, referenceNumber, logoBytes);
            }
            else
            {
                pdfBytes = GenerateLetterPdf(dossier, demande, referenceNumber, logoBytes);
            }

            if (existingAttestation != null)
            {
                existingAttestation.Donnees = pdfBytes;
                existingAttestation.DateDelivrance = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                existingAttestation.DateExpiration = demande.EstHomologable
                    ? DateTimeOffset.UtcNow.AddYears(3).ToUnixTimeMilliseconds()
                    : DateTimeOffset.UtcNow.AddYears(10).ToUnixTimeMilliseconds();
                existingAttestation.Extension = "pdf";
                _context.Attestations.Update(existingAttestation);
            }
            else
            {
                var newAttestation = new Attestation
                {
                    Id = Guid.NewGuid(),
                    IdDemande = demande.Id,
                    DateDelivrance = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    DateExpiration = demande.EstHomologable
                        ? DateTimeOffset.UtcNow.AddYears(3).ToUnixTimeMilliseconds()
                        : DateTimeOffset.UtcNow.AddYears(10).ToUnixTimeMilliseconds(),
                    Donnees = pdfBytes,
                    Extension = "pdf",
                    NumeroSequentiel = seqNumber
                };
                _context.Attestations.Add(newAttestation);
            }
        }

        await _context.SaveChangesAsync(default);
        _logger.LogInformation("Documents générés pour le dossier {Numero}.", dossier.Numero);
    }

    // --- CERTIFICAT ---
    private byte[] GenerateCertificatePdf(Dossier dossier, Demande demande, string referenceNumber, byte[] logoBytes)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Row(row =>
                {
                    // Colonne Logo (Gauche)
                    if (logoBytes.Length > 0)
                    {
                        row.ConstantItem(80).Image(logoBytes); 
                    }

                    // Colonne Titre (Droite/Centre)
                    row.RelativeItem().PaddingLeft(10).Column(col =>
                    {
                        col.Item().Text("Agence de Régulation des Postes et des Communications Électroniques").FontSize(10).FontColor(Colors.Green.Medium);
                        col.Item().AlignCenter().Text("CERTIFICAT D'HOMOLOGATION").FontSize(18).Bold();
                        col.Item().AlignCenter().Text(referenceNumber).FontSize(12).Bold();
                        col.Item().AlignCenter().Text("----------o00o----------").FontSize(10);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Text(_settings.LegalReference).Justify();
                    col.Item().Height(20);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns => { columns.ConstantColumn(120); columns.RelativeColumn(); });
                        table.Cell().Text("Equipement :").Bold(); table.Cell().Text(demande.Equipement);
                        table.Cell().Text("Modèle :").Bold(); table.Cell().Text(demande.Modele);
                        table.Cell().Text("Marque :").Bold(); table.Cell().Text(demande.Marque);
                        table.Cell().Text("Fabricant :").Bold(); table.Cell().Text(demande.Fabricant);
                    });

                    col.Item().Height(20);
                    col.Item().Text($"Nom de la société (demandeur) : {dossier.Client?.RaisonSociale}").Bold();
                    col.Item().Text($"Adresse : {dossier.Client?.Adresse ?? "N/A"}").Bold();

                    col.Item().Height(30);
                    col.Item().Text(_settings.ValidityText);

                    col.Item().Height(30);
                    col.Item().AlignRight().Text($"Fait à Brazzaville, le {DateTime.Now:dd MMMM yyyy}");
                    col.Item().AlignRight().PaddingTop(10).Text("Le Directeur Général,").Bold();
                    col.Item().AlignRight().PaddingTop(40).Text(_settings.DirectorGeneralName).Bold();
                });

                page.Footer().AlignCenter().Column(c =>
                {
                    c.Item().LineHorizontal(1).LineColor(Colors.Black);
                    c.Item().Text($"{_settings.ArpceAddress} | {_settings.ArpceEmail}").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });
        return document.GeneratePdf();
    }

    // --- LETTRE (Avec Logo) ---
    private byte[] GenerateLetterPdf(Dossier dossier, Demande demande, string referenceNumber, byte[] logoBytes)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Row(row =>
                {
                    // Colonne Gauche : Logo + Référence en dessous
                    row.RelativeItem().Column(col =>
                    {
                        if (logoBytes.Length > 0)
                        {
                            col.Item().Width(80).Image(logoBytes);
                        }
                        col.Item().PaddingTop(5).Text(referenceNumber).FontSize(10).Bold();
                    });

                    // Colonne Droite : Destinataire
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Pointe-Noire, le {DateTime.Now:dd MMMM yyyy}");
                        col.Item().Height(20);
                        col.Item().Text("Monsieur le Directeur Général").Bold();
                        col.Item().Text($"de la Société {dossier.Client?.RaisonSociale}").Bold();
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Text("Objet : Homologation des équipements").Bold().Underline();
                    col.Item().Height(20);

                    col.Item().Text("Monsieur le Directeur Général,");
                    col.Item().Height(10);
                    col.Item().Text("J'accuse réception de votre correspondance...");
                    col.Item().Height(10);
                    col.Item().Text("Je vous informe que cet équipement n'est pas soumis à homologation :");

                    col.Item().PaddingLeft(20).Row(row =>
                    {
                        row.ConstantItem(10).Text("▪");
                        row.RelativeItem().Text($"{demande.Equipement} {demande.Modele}").Bold();
                    });

                    col.Item().Height(20);
                    col.Item().AlignRight().Text("Le Chef d'Antenne Départementale").Bold();
                    col.Item().AlignRight().PaddingTop(40).Text(_settings.AntennaChiefName).Bold();
                });

                page.Footer().AlignCenter().Column(c =>
                {
                    c.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    c.Item().Text($"{_settings.AntennaPOBox} | Email : {_settings.ArpceEmail}").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });
        return document.GeneratePdf();
    }
}