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
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

public class ReceiptGeneratorService : IReceiptGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorage;
    private readonly CertificateSettings _settings;

    public ReceiptGeneratorService(IApplicationDbContext context, IFileStorageProvider fileStorage, IConfiguration configuration)
    {
        _context = context;
        _fileStorage = fileStorage;
        _settings = configuration.GetSection("CertificateSettings").Get<CertificateSettings>() ?? new CertificateSettings();
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
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Agence de Régulation des Postes et des Communications Électroniques").FontSize(10).FontColor("#4E9741").Bold();
                        col.Item().Text("DIRECTION ADMINISTRATIVE ET FINANCIÈRE").FontSize(9).Italic();
                    });
                    row.ConstantItem(120).Text($"REÇU N°: {quittance}").Bold().FontSize(12);
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().AlignCenter().Text("REÇU DE PAIEMENT").FontSize(18).Bold().Underline();
                    col.Item().Height(20);
                    col.Item().Text($"Reçu de : {dossier.Client?.RaisonSociale}");
                    col.Item().Text($"La somme de : {montant:N0} FCFA").Bold();
                    col.Item().Text($"Objet : Règlement dossier {dossier.Numero}");
                    col.Item().Text($"Mode : {mode}");
                    col.Item().PaddingTop(40).AlignRight().Text("Le Caissier,").Bold();
                });
            });
        }).GeneratePdf();

        await _fileStorage.SaveDocumentDossierFromBytesAsync(pdf, $"Recu_{dossier.Numero}.pdf", 3, dossier.Id, "Reçu de paiement caisse");

        return pdf;
    }
}