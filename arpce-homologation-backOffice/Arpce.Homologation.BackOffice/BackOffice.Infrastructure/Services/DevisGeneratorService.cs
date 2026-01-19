using BackOffice.Application.Common;
using BackOffice.Application.Common.Exceptions;
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

public class DevisGeneratorService : IDevisGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly CertificateSettings _settings;
    private readonly ILogger<DevisGeneratorService> _logger;
    private readonly string _logoPath;

    public DevisGeneratorService(
        IApplicationDbContext context,
        IConfiguration configuration,
        ILogger<DevisGeneratorService> logger)
    {
        _context = context;
        _logger = logger;
        _settings = configuration.GetSection("CertificateSettings").Get<CertificateSettings>() ?? new CertificateSettings();
        _logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo_arpce.png");
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task GenerateAndSaveDevisPdfAsync(Guid devisId)
    {
        var devis = await _context.Devis
            .AsNoTracking()
            .Include(d => d.Dossier).ThenInclude(dos => dos.Client)
            .FirstOrDefaultAsync(d => d.Id == devisId);

        if (devis == null)
        {
            _logger.LogError("Devis introuvable pour génération PDF : {DevisId}", devisId);
            throw new FileNotFoundException("Devis introuvable.");
        }

        byte[] logoBytes = File.Exists(_logoPath) ? await File.ReadAllBytesAsync(_logoPath) : Array.Empty<byte>();

        var total = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0);

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Row(row =>
                {
                    if (logoBytes.Length > 0) row.ConstantItem(80).Image(logoBytes);
                    row.RelativeItem().PaddingLeft(20).Column(col =>
                    {
                        col.Item().Text("Agence de Régulation des Postes et des Communications Électroniques").Bold().FontSize(12);
                        col.Item().Text("Direction Administrative, Financière et Comptable").FontSize(10);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().AlignCenter().Text("DEVIS").Bold().FontSize(20).Underline();
                    col.Item().Height(20);
                    col.Item().AlignRight().Text($"Brazzaville, le {devis.Date.FromUnixTimeMilliseconds():dd MMMM yyyy}");
                    col.Item().Height(20);
                    col.Item().Text(text => { text.Span("Client : ").SemiBold(); text.Span(devis.Dossier.Client.RaisonSociale); });
                    col.Item().Text(text => { text.Span("Dossier N° : ").SemiBold(); text.Span(devis.Dossier.Numero); });
                    col.Item().Text(text => { text.Span("Objet : ").SemiBold(); text.Span(devis.Dossier.Libelle); });
                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns => { columns.RelativeColumn(3); columns.RelativeColumn(1); });
                        table.Header(header => { header.Cell().Border(1).Padding(5).Text("Désignation").Bold(); header.Cell().Border(1).Padding(5).AlignRight().Text("Montant (XAF)").Bold(); });

                        table.Cell().Border(1).Padding(5).Text("Frais d'étude de dossier");
                        table.Cell().Border(1).Padding(5).AlignRight().Text($"{devis.MontantEtude:N0}");

                        table.Cell().Border(1).Padding(5).Text("Frais d'homologation des équipements");
                        table.Cell().Border(1).Padding(5).AlignRight().Text($"{devis.MontantHomologation:N0}");

                        table.Cell().Border(1).Padding(5).Text("Frais de contrôle sur site");
                        table.Cell().Border(1).Padding(5).AlignRight().Text($"{devis.MontantControle:N0}");

                        table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3).Text("MONTANT TOTAL HT").Bold();
                        table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3).AlignRight().Text($"{total:N0}").Bold();
                    });

                    col.Item().PaddingTop(20).Text($"Arrêté le présent devis à la somme de : {NumberToWords((long)total)} francs CFA.").Italic();
                });

                page.Footer().AlignCenter().Column(c =>
                {
                    c.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    c.Item().PaddingTop(5).Text($"{_settings.ArpceAddress} | BP: {_settings.ArpcePOBox} | Email: {_settings.ArpceEmail}").FontSize(8);
                });
            });
        }).GeneratePdf();

        var existingDocument = await _context.DocumentsDossiers
            .FirstOrDefaultAsync(d => d.IdDossier == devis.IdDossier && d.Type == 2);

        if (existingDocument != null)
        {
            existingDocument.Donnees = pdfBytes;
            existingDocument.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _context.DocumentsDossiers.Update(existingDocument);
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
                DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                UtilisateurCreation = "SYSTEM"
            });
        }
        await _context.SaveChangesAsync(default);
    }

    private string NumberToWords(long number) { return number.ToString("N0"); }
}