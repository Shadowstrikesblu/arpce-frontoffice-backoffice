using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

public class ReceiptGeneratorService : IReceiptGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly CertificateSettings _settings;
    private readonly string _logoPath;

    public ReceiptGeneratorService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _settings = configuration.GetSection("CertificateSettings").Get<CertificateSettings>() ?? new CertificateSettings();
        _logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo_arpce.png");
    }

    public async Task<byte[]> GenerateReceiptPdfAsync(Guid dossierId, decimal montant, string modePaiement, string numeroQuittance)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Client)
            .Include(d => d.Demande)
            .FirstOrDefaultAsync(d => d.Id == dossierId);

        if (dossier == null) throw new Exception("Dossier introuvable pour la génération du reçu.");

        byte[] logoBytes = File.Exists(_logoPath) ? await File.ReadAllBytesAsync(_logoPath) : Array.Empty<byte>();

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Row(row =>
                {
                    if (logoBytes.Length > 0) row.ConstantItem(80).Image(logoBytes);
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Agence de Régulation des Postes et des Communications Électroniques").FontSize(10).FontColor("#4E9741").Bold();
                        col.Item().Text("DIRECTION ADMINISTRATIVE ET FINANCIÈRE").FontSize(9).Italic();
                    });
                    row.ConstantItem(120).Column(col => {
                        col.Item().Text($"REÇU N°: {numeroQuittance}").Bold().FontSize(12);
                        col.Item().Text($"Date: {DateTime.Now:dd/MM/yyyy}").FontSize(10);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().AlignCenter().Text("REÇU DE PAIEMENT CAISSE").FontSize(18).Bold().Underline();
                    col.Item().Height(20);

                    col.Item().Text(t => {
                        t.Span("Reçu de : ").Bold();
                        t.Span(dossier.Client?.RaisonSociale ?? "N/A");
                    });

                    col.Item().PaddingTop(10).Text(t => {
                        t.Span("La somme de : ").Bold();
                        t.Span($"{montant:N0} FCFA").FontSize(14).Bold();
                    });

                    col.Item().PaddingTop(10).Text(t => {
                        t.Span("Objet : ").Bold();
                        t.Span($"Règlement des frais d'homologation pour le dossier {dossier.Numero}");
                    });

                    col.Item().PaddingTop(10).Text(t => {
                        t.Span("Mode de règlement : ").Bold();
                        t.Span(modePaiement);
                    });

                    col.Item().Height(30);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns => {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });
                        table.Header(header => {
                            header.Cell().Border(1).Padding(5).Text("Désignation");
                            header.Cell().Border(1).Padding(5).AlignCenter().Text("Montant (FCFA)");
                        });
                        table.Cell().Border(1).Padding(5).Text($"Frais d'homologation - {dossier.Demande?.Equipement}");
                        table.Cell().Border(1).Padding(5).AlignCenter().Text($"{montant:N0}");
                    });

                    col.Item().PaddingTop(40).AlignRight().Column(sig => {
                        sig.Item().Text("Le Caissier,").Bold();
                        sig.Item().Height(50);
                        sig.Item().Text("Cachet et Signature").Italic().FontSize(9);
                    });
                });

                page.Footer().AlignCenter().Text(x => {
                    x.Span("ARPCE - ").FontSize(9);
                    x.Span(_settings.ArpceAddress).FontSize(9);
                });
            });
        }).GeneratePdf();
    }
}