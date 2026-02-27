using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

public class ReceiptGeneratorService : IReceiptGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorage;

    public ReceiptGeneratorService(IApplicationDbContext context, IFileStorageProvider fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<byte[]> GenerateReceiptPdfAsync(Guid dossierId, decimal montant, string mode, string quittance)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Client)
            .Include(d => d.Demande)
            .FirstOrDefaultAsync(d => d.Id == dossierId);

        if (dossier == null) throw new Exception("Dossier introuvable.");

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Taille adaptée au format carnet (Paysage)
                page.Size(PageSizes.A5.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);

                // Style global : Bordures et texte en bleu foncé comme sur l'image
                var themeColor = "#2A3E82";
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial).FontColor(themeColor));

                page.Content().Column(col =>
                {
                    // --- PARTIE HAUTE ---
                    col.Item().Row(row =>
                    {
                        // 1. Cadre de Gauche (Entité : CREP)
                        row.RelativeItem().Border(1.5f).BorderColor(themeColor).Height(60).AlignCenter().AlignMiddle()
                           .Text("CREP").FontSize(30).ExtraBold();

                        row.ConstantItem(15);

                        // 2. Blocs de Droite empilés
                        row.ConstantItem(220).Column(c =>
                        {
                            // Ligne REÇU No.
                            c.Item().Row(r => {
                                r.RelativeItem().Border(1.5f).BorderColor(themeColor).Padding(5).Text("REÇU No.");
                                r.ConstantItem(80).Border(1.5f).BorderColor(themeColor).Padding(5).AlignCenter().Text(quittance).Bold().FontColor(Colors.Red.Medium);
                            });
                            // Ligne MONTANT
                            c.Item().Row(r => {
                                r.RelativeItem().Border(1.5f).BorderColor(themeColor).Padding(5).Text("MONTANT");
                                r.ConstantItem(120).Border(1.5f).BorderColor(themeColor).Padding(5).AlignRight().Text($"{montant:N0} XAF").Bold();
                            });
                            // Ligne DATE
                            c.Item().Row(r => {
                                r.RelativeItem().Border(1.5f).BorderColor(themeColor).Padding(5).Text("DATE");
                                r.ConstantItem(120).Border(1.5f).BorderColor(themeColor).Padding(5).AlignCenter().Text(DateTime.Now.ToString("dd/MM/yyyy"));
                            });
                        });
                    });

                    col.Item().Height(10);

                    // --- PARTIE CORPS DU REÇU ---
                    col.Item().Border(1.5f).BorderColor(themeColor).Padding(15).Layers(layers =>
                    {
                        // Tampon PAYÉ en filigrane (Exactement comme l'image)
                        layers.Layer().AlignCenter().AlignMiddle().Rotate(-25).Text("PAYÉ")
                            .FontSize(70).ExtraBold().FontColor("#E0E0E0");

                        layers.PrimaryLayer().Column(c =>
                        {
                            // Ligne 1 : Reçu de
                            c.Item().PaddingVertical(5).Text(t => {
                                t.Span("Reçu de ").FontSize(14);
                                t.Span($" {dossier.Client?.RaisonSociale} ").Bold().FontSize(15);
                            });

                            // Ligne 2 : La somme
                            c.Item().AlignRight().Text("la somme").FontSize(11).Italic();
                            c.Item().PaddingVertical(5).Text(t => {
                                t.Span("de ").FontSize(14);
                                t.Span($" {NumberToFrenchWords((long)montant).ToUpper()} FRANCS CFA ").Bold().FontSize(13);
                            });

                            // Ligne 3 : Objet
                            c.Item().PaddingVertical(5).Text(t => {
                                t.Span("Règlement de la ").FontSize(14);
                                t.Span($" NOTE No {dossier.Numero} / Frais d'Homologation ").Bold().FontSize(13);
                            });

                            // Bloc Signature en bas
                            c.Item().Row(r => {
                                r.RelativeItem(); // Pousse à droite
                                r.ConstantItem(160).PaddingTop(10).Border(1.5f).BorderColor(themeColor).Row(sr => {
                                    sr.RelativeItem().Padding(5).Text("Signature").FontSize(11).Italic();
                                    sr.ConstantItem(100).Height(40); // Espace vide pour signature
                                });
                            });
                        });
                    });
                });
            });
        }).GeneratePdf();

        await _fileStorage.SaveDocumentDossierFromBytesAsync(pdf, $"Recu_{dossier.Numero}.pdf", 3, dossier.Id, "Reçu de paiement");
        return pdf;
    }

    // Algorithme de conversion pour la somme en toutes lettres
    private string NumberToFrenchWords(long number)
    {
        if (number == 0) return "zéro";
        if (number < 0) return "moins " + NumberToFrenchWords(Math.Abs(number));
        string words = "";
        if ((number / 1000000) > 0)
        {
            long millions = number / 1000000;
            words += (millions == 1 ? "un million " : NumberToFrenchWords(millions) + " millions ");
            number %= 1000000;
        }
        if ((number / 1000) > 0)
        {
            long thousands = number / 1000;
            words += (thousands == 1 ? "mille " : NumberToFrenchWords(thousands) + " mille ");
            number %= 1000;
        }
        if ((number / 100) > 0)
        {
            long hundreds = number / 100;
            words += (hundreds == 1 ? "cent " : NumberToFrenchWords(hundreds) + " cents ");
            number %= 100;
        }
        if (number > 0)
        {
            var unitsMap = new[] { "zéro", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize", "dix-sept", "dix-huit", "dix-neuf" };
            var tensMap = new[] { "zéro", "dix", "vingt", "trente", "quarante", "cinquante", "soixante", "soixante-dix", "quatre-vingt", "quatre-vingt-dix" };
            if (number < 20) words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0) words += "-" + unitsMap[number % 10];
            }
        }
        return words.Trim();
    }
}