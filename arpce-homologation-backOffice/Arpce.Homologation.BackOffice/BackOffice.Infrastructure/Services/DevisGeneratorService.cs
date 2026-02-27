using BackOffice.Application.Common;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

public class DevisGeneratorService : IDevisGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly CertificateSettings _settings;
    private readonly ILogger<DevisGeneratorService> _logger;
    private readonly string _logoPath;

    // Variable pour stocker le nom de la police à utiliser (Poppins ou Arial)
    private readonly string _fontName;

    public DevisGeneratorService(IApplicationDbContext context, IConfiguration configuration, ILogger<DevisGeneratorService> logger)
    {
        _context = context;
        _logger = logger;
        _settings = configuration.GetSection("CertificateSettings").Get<CertificateSettings>() ?? new CertificateSettings();
        _logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo_arpce.png");
        QuestPDF.Settings.License = LicenseType.Community;

        // --- LOGIQUE DE CHARGEMENT DE LA POLICE ---
        // Par défaut, on utilise Arial
        _fontName = Fonts.Arial;

        try
        {
            var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Poppins-Regular.ttf");

            // On vérifie simplement si le fichier existe
            if (File.Exists(fontPath))
            {
                // On charge la police dans QuestPDF
                using var fontStream = File.OpenRead(fontPath);
                FontManager.RegisterFont(fontStream);

                // Si le chargement réussit, on définit la police sur "Poppins"
                _fontName = "Poppins";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de charger la police Poppins. Utilisation d'Arial par défaut.");
            _fontName = Fonts.Arial;
        }
    }

    public async Task GenerateAndSaveDevisPdfAsync(Guid devisId)
    {
        var devis = await _context.Devis
            .AsNoTracking()
            .Include(d => d.Dossier).ThenInclude(dos => dos.Client)
            .Include(d => d.Demande)
            .FirstOrDefaultAsync(d => d.Id == devisId);

        if (devis == null) throw new Exception("Devis introuvable.");

        byte[] logoBytes = File.Exists(_logoPath) ? await File.ReadAllBytesAsync(_logoPath) : Array.Empty<byte>();

        // Calcul des montants
        decimal montantTotal = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0) + devis.MontantPenalite;
        decimal remise = 0;
        decimal netAPayer = montantTotal - remise;

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);

                // Application de la police conditionnelle (_fontName)
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(_fontName).FontColor(Colors.Black));

                // --- HEADER (LOGO + NOM AGENCE + TABLEAU NOTE) ---
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(90).Column(c => {
                            if (logoBytes.Length > 0) c.Item().Image(logoBytes);
                            c.Item().PaddingTop(2).AlignCenter().Text("République du Congo").FontSize(7).Italic();
                        });

                        row.RelativeItem().PaddingTop(15).AlignCenter().Column(c =>
                        {
                            var agencyColor = "#2E7D6A";
                            c.Item().Text("Agence de Régulation des Postes").FontSize(18).FontColor(agencyColor);
                            c.Item().Text("et des Communications Électroniques").FontSize(18).FontColor(agencyColor);
                        });
                    });

                    col.Item().Height(20);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(devis.Dossier.Client?.RaisonSociale?.ToUpper()).FontSize(11).ExtraBold();
                            c.Item().Text(devis.Dossier.Client?.Adresse ?? "N/A");
                            c.Item().Text(devis.Dossier.Client?.Ville?.ToUpper() ?? "CONGO");
                        });

                        row.ConstantItem(220).Table(t =>
                        {
                            t.ColumnsDefinition(columns => {
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(0.6f);
                            });
                            t.Cell().Row(1).ColumnSpan(3).Border(0.5f).Padding(3).AlignCenter()
                                .Text($"Note N°: {devis.Dossier.Numero}/ARPCE/DAFC/DRSCE/SNH/{DateTime.Now:yy}").Bold();
                            t.Cell().Row(2).Column(1).Border(0.5f).Padding(1).AlignCenter().Text("DATE").Bold().FontSize(7);
                            t.Cell().Row(2).Column(2).Border(0.5f).Padding(1).AlignCenter().Text("REDEVABLE").Bold().FontSize(7);
                            t.Cell().Row(2).Column(3).Border(0.5f).Padding(1).AlignCenter().Text("PAGE").Bold().FontSize(7);
                            t.Cell().Row(3).Column(1).Border(0.5f).Padding(3).AlignCenter().Text($"{DateTime.Now:dd/MM/yyyy}");
                            t.Cell().Row(3).Column(2).Border(0.5f).Padding(3).AlignCenter().Text(devis.Dossier.Client?.Code?.Replace("CLT-", "") ?? "N/A");
                            t.Cell().Row(3).Column(3).Border(0.5f).Padding(3).AlignCenter().Text("1 / 1");
                        });
                    });
                });

                // --- CONTENU (TABLEAU DES CONDITIONS + ÉQUIPEMENTS) ---
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(columns => { columns.RelativeColumn(2); columns.RelativeColumn(1); columns.RelativeColumn(2); });
                        t.Cell().Border(0.5f).PaddingLeft(5).Text("Objet : Frais d'étude de dossier et frais d'homologation").Bold();
                        t.Cell().Border(0.5f).AlignCenter().Text("Date d'échéance").Bold();
                        t.Cell().Border(0.5f).PaddingLeft(5).Text("Conditions de Règlement: Chèque, Espèces, Virement").Bold();
                        t.Cell().Border(0.5f).PaddingLeft(5).Text("Type de Service: Homologation");
                        t.Cell().Border(0.5f).AlignCenter().Text("Immédiat");
                        t.Cell().Border(0.5f).PaddingLeft(5).Text(text => {
                            text.Span("A l'ordre l'ARPCE compte : ");
                            text.Span("BCH 30015 24201 10100000594/63").FontColor(Colors.Red.Medium).Bold();
                        });
                    });

                    col.Item().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns => {
                            columns.RelativeColumn(6); columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(2);
                        });

                        table.Header(header => {
                            var headerBg = "#CCCCCC";
                            header.Cell().Background(headerBg).Border(0.5f).Padding(5).AlignCenter().Text("Désignation").Bold();
                            header.Cell().Background(headerBg).Border(0.5f).Padding(5).AlignCenter().Text("Qté").Bold();
                            header.Cell().Background(headerBg).Border(0.5f).Padding(5).AlignCenter().Text("P.U(F.Cfa)").Bold();
                            header.Cell().Background(headerBg).Border(0.5f).Padding(5).AlignCenter().Text("Montant HT").Bold();
                        });

                        // Ligne 1 : Étude
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).Text("Frais d'étude de dossier pour l'homologation des équipements des télécommunications.");
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignCenter().Text("1");
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text($"{devis.MontantEtude:N0}");
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text($"{devis.MontantEtude:N0}");

                        // Ligne 2 : Homologation
                        var eq = devis.Demande;
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).PaddingTop(10).Text($"Frais d'homologation des équipements de télécommunications de type {eq?.Type}, de modèle {eq?.Modele} et de marque {eq?.Marque?.ToUpper()}.");
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignCenter().Text($"{eq?.QuantiteEquipements ?? 1}");
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text($"{devis.MontantHomologation / (eq?.QuantiteEquipements ?? 1):N0}");
                        table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text($"{devis.MontantHomologation:N0}");

                        // --- LIGNE DES FRAIS DE CONTRÔLE ---
                        if (devis.MontantControle > 0)
                        {
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).PaddingTop(10).Text("Frais de contrôle de conformité.");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignCenter().Text("1");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text($"{devis.MontantControle:N0}");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text($"{devis.MontantControle:N0}");
                        }

                        // Ligne Pénalité (si applicable)
                        if (devis.MontantPenalite > 0)
                        {
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).Text("Pénalité de retard (10% selon règlementation)").FontColor(Colors.Red.Medium).Bold();
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignCenter().Text("-");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text("-");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Padding(5).AlignRight().Text($"{devis.MontantPenalite:N0}").FontColor(Colors.Red.Medium).Bold();
                        }

                        // Remplissage lignes vides pour étirer le tableau
                        for (int i = 0; i < 6; i++)
                        {
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Height(18).Text("");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Text("");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Text("");
                            table.Cell().BorderLeft(0.5f).BorderRight(0.5f).Text("");
                        }
                        table.Cell().ColumnSpan(4).BorderTop(0.5f).Height(1);
                    });

                    // --- SECTION : REMARQUES ET RÉCAPITULATIF  ---
                    col.Item().PaddingTop(5).Column(c =>
                    {
                        c.Item().Text("Remarque: Cette note de recouvrement est établie sur une base \"hors taxes\". Si une taxation devrait être établie,").FontSize(8);
                        c.Item().PaddingLeft(45).Text("une note complémentaire vous sera adressée.").FontSize(8);

                        c.Item().PaddingTop(10).Text(t => {
                            t.Span("Selon l'arrêté 1279, article 13 le non-paiement du montant exigé au-delà de 30 jours").Bold().FontSize(8);
                        });
                        c.Item().Text(t => {
                            t.Span("entraînera l'application d'une pénalité de ").Bold().FontSize(8);
                            t.Span("10%").Bold().FontSize(8).FontColor(Colors.Red.Medium);
                        });

                        // Tableau Récapitulatif
                        c.Item().PaddingTop(15).Table(summaryTable =>
                        {
                            summaryTable.ColumnsDefinition(columns => {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            summaryTable.Cell().Border(0.5f).Background("#f2f2f2").Padding(3).AlignCenter().Text("MONTANT TOTAL").Bold();
                            summaryTable.Cell().Border(0.5f).Background("#f2f2f2").Padding(3).AlignCenter().Text("REMISE").Bold();
                            summaryTable.Cell().Border(0.5f).Background("#f2f2f2").Padding(3).AlignCenter().Text("NET A PAYER").Bold();

                            summaryTable.Cell().Border(0.5f).Padding(5).AlignCenter().Text($"{montantTotal:N0}").Bold();
                            summaryTable.Cell().Border(0.5f).Padding(5).AlignCenter().Text($"{remise:N0}").Bold();
                            summaryTable.Cell().Border(0.5f).Padding(5).AlignCenter().Text($"{netAPayer:N0}").Bold().FontColor(Colors.Red.Medium);
                        });

                        // Arrêté la présente note
                        c.Item().PaddingTop(15).Text(t => {
                            t.Span("Arrêté la présente note à la somme de : ").FontSize(9);
                            t.Span($"{NumberToFrenchWords((long)netAPayer)} Fcfa").Bold().FontSize(9);
                        });
                    });
                });

                // --- FOOTER INSTITUTIONNEL (Alignement centré ligne par ligne) ---
                page.Footer().AlignCenter().Column(footer =>
                {
                    // La double ligne décorative (Marron/Rouge)
                    footer.Item().PaddingBottom(5).Column(c => {
                        c.Item().LineHorizontal(1.5f).LineColor("#8B4513");
                        c.Item().PaddingTop(1).LineHorizontal(0.5f).LineColor("#8B4513");
                    });

                    // Textes centrés ligne par ligne 
                    footer.Item().AlignCenter().Text("Etablissement public Administratif doté de la personnalité juridique et de l'autonomie").FontSize(7.5f);
                    footer.Item().AlignCenter().Text("Financière placé sous la tutelle du MPTEN").FontSize(7.5f);
                    footer.Item().AlignCenter().Text("Adresse: Immeuble ARPCE, 91 Avenue de l'Amitié -BP. 2490 Centre-Ville, Brazzaville-Congo").FontSize(7.5f);
                    footer.Item().AlignCenter().Text(t => {
                        t.Span("Téléphone : +242 05 510 72 72 | Email : ").FontSize(7.5f);
                        t.Span("contact@arpce.cg").FontSize(7.5f).FontColor(Colors.Blue.Medium).Underline();
                    });
                });
            });
        }).GeneratePdf();

        // Logique de stockage 
        var existingDoc = await _context.DocumentsDossiers.FirstOrDefaultAsync(d => d.IdDossier == devis.IdDossier && d.Type == 2);
        if (existingDoc != null)
        {
            existingDoc.Donnees = pdfBytes;
            existingDoc.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        else
        {
            _context.DocumentsDossiers.Add(new DocumentDossier
            {
                Id = Guid.NewGuid(),
                IdDossier = devis.IdDossier,
                Nom = $"Devis_{devis.Dossier.Numero}",
                Type = 2,
                Extension = "pdf",
                Donnees = pdfBytes,
                DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }
        await _context.SaveChangesAsync(default);
    }

    private string NumberToFrenchWords(long number) { return number.ToString("N0"); }
}