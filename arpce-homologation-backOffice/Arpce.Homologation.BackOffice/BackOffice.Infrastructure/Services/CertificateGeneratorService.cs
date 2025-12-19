using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common.Models;
using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

public class CertificateGeneratorService : ICertificateGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<CertificateGeneratorService> _logger;
    private readonly CertificateSettings _settings;
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

        int maxSequence = await _context.Attestations
            .Where(a => a.DateDelivrance >= new DateTimeOffset(new DateTime(currentYear, 1, 1)).ToUnixTimeMilliseconds())
            .MaxAsync(a => (int?)a.NumeroSequentiel) ?? 0;

        int nextSequence = maxSequence + 1;

        byte[] logoBytes = File.Exists(_logoPath) ? await File.ReadAllBytesAsync(_logoPath) : Array.Empty<byte>();

        foreach (var demande in dossier.Demandes)
        {
            var existingAttestation = await _context.Attestations.FirstOrDefaultAsync(a => a.IdDemande == demande.Id);

            int seqNumber = existingAttestation?.NumeroSequentiel ?? nextSequence;
            if (existingAttestation == null) nextSequence++;

            string formattedSeq = seqNumber.ToString("D4");

            // --- CORRECTION DU FORMAT DU NUMÉRO ---
            string referenceNumber = $"N°/{formattedSeq}/ARPCE-DG/{currentYear}";
            // ------------------------------------

            byte[] pdfBytes = demande.EstHomologable
                ? GenerateCertificatePdf(dossier, demande, referenceNumber, logoBytes)
                : GenerateLetterPdf(dossier, demande, referenceNumber, logoBytes);

            if (existingAttestation != null)
            {
                existingAttestation.Donnees = pdfBytes;
                existingAttestation.DateDelivrance = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                existingAttestation.DateExpiration = demande.EstHomologable ? DateTimeOffset.UtcNow.AddYears(3).ToUnixTimeMilliseconds() : DateTimeOffset.UtcNow.AddYears(10).ToUnixTimeMilliseconds();
                existingAttestation.Extension = "pdf";
                _context.Attestations.Update(existingAttestation);
            }
            else
            {
                _context.Attestations.Add(new Attestation
                {
                    Id = Guid.NewGuid(),
                    IdDemande = demande.Id,
                    DateDelivrance = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    DateExpiration = demande.EstHomologable ? DateTimeOffset.UtcNow.AddYears(3).ToUnixTimeMilliseconds() : DateTimeOffset.UtcNow.AddYears(10).ToUnixTimeMilliseconds(),
                    Donnees = pdfBytes,
                    Extension = "pdf",
                    NumeroSequentiel = seqNumber
                });
            }
        }

        await _context.SaveChangesAsync(default);
        _logger.LogInformation("Documents générés pour le dossier {Numero}.", dossier.Numero);
    }

    private void DrawColorBar(IContainer container)
    {
        container.Row(row =>
        {
            row.ConstantItem(5, Unit.Millimetre).Background("#009A44"); // Vert
            row.ConstantItem(5, Unit.Millimetre).Background("#F8CD29"); // Jaune
            row.ConstantItem(5, Unit.Millimetre).Background("#CE2A2D"); // Rouge
        });
    }

    private byte[] GenerateCertificatePdf(Dossier dossier, Demande demande, string referenceNumber, byte[] logoBytes)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("New Century Schoolbook"));

                page.Background().Row(row =>
                {
                    // --- CORRECTION DE LA MARGE ---
                    row.ConstantItem(1, Unit.Centimetre); // Marge de 1cm
                    // ------------------------------
                    DrawColorBar(row.ConstantItem(15, Unit.Millimetre));
                    row.RelativeItem();
                });

                // On ajuste le padding gauche du contenu pour qu'il soit bien aligné
                page.Content().PaddingLeft(3f, Unit.Centimetre).PaddingRight(2, Unit.Centimetre).PaddingVertical(1.5f, Unit.Centimetre).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        if (logoBytes.Length > 0) row.ConstantItem(80).Image(logoBytes);
                        row.RelativeItem().PaddingLeft(10).Column(c =>
                        {
                            c.Item().Text("Agence de Régulation des Postes et des Communications Électroniques").FontSize(10).FontColor("#4E9741");
                            c.Item().AlignCenter().PaddingTop(10).Text("CERTIFICAT D'HOMOLOGATION").FontSize(16).Bold();
                            c.Item().AlignCenter().Text(referenceNumber).FontSize(12).Bold();
                            c.Item().AlignCenter().Text("----------o00o----------").FontSize(10);
                        });
                    });

                    col.Item().PaddingTop(20).Text(_settings.LegalReference).Justify();
                    col.Item().Height(15);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns => { columns.ConstantColumn(180); columns.RelativeColumn(); });
                        void AddRow(string label, string? value) { table.Cell().PaddingBottom(2).Text(label).Bold(); table.Cell().PaddingBottom(2).Text(value ?? "N/A"); }
                        AddRow("Equipement", $": {demande.Equipement}");
                        AddRow("Modèle", $": {demande.Modele}");
                        AddRow("Marque", $": {demande.Marque}");
                        AddRow("Description", $": {demande.Description}");
                        AddRow("Fabricant", $": {demande.Fabricant}");
                    });

                    col.Item().PaddingTop(10).Text("Documents techniques présentés : Schémas techniques, description technique, rapport de test,").FontSize(11);
                    col.Item().Height(15);

                    col.Item().Text($"Nom de la société (demandeur) : {dossier.Client?.RaisonSociale}").Bold();
                    col.Item().Text($"Nom de la société (représentant en République du Congo) : {dossier.Client?.RaisonSociale}").Bold();
                    col.Item().Text($"Adresse : {dossier.Client?.Adresse ?? "N/A"}").Bold();
                    col.Item().Text($"Tél : {dossier.Client?.ContactTelephone}   E-mail : {dossier.Client?.Email}").Bold();

                    col.Item().Height(25);
                    col.Item().Text(_settings.ValidityText);
                    col.Item().Text("Un certificat de conformité à la réglementation sera établi pour servir et valoir ce que de droit.");

                    col.Item().ExtendVertical().AlignBottom().AlignRight().Column(sig =>
                    {
                        sig.Item().Text($"Fait à Brazzaville, le {DateTime.Now:dd MMMM yyyy}");
                        sig.Item().PaddingTop(20).Text("Le Directeur Général,").Bold();
                        sig.Item().PaddingTop(60).Text(_settings.DirectorGeneralName).Bold();
                    });
                });

                page.Footer().PaddingHorizontal(2.5f, Unit.Centimetre).PaddingBottom(1, Unit.Centimetre).AlignCenter().Column(c =>
                {
                    c.Item().LineHorizontal(1).LineColor(Colors.Black);
                    c.Item().PaddingTop(5).Text("Etablissement public administratif doté de la personnalité juridique et de l'autonomie financière placé").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"Sous la tutelle du MPTEN | Adresse: {_settings.ArpceAddress}").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"{_settings.ArpcePOBox} | Téléphone : +242.05.510.72.72 | Email : {_settings.ArpceEmail}").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }

    private byte[] GenerateLetterPdf(Dossier dossier, Demande demande, string referenceNumber, byte[] logoBytes)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Background().Row(row =>
                {
                    // --- CORRECTION DE LA MARGE ---
                    row.ConstantItem(1, Unit.Centimetre); // Marge de 1cm
                    // ------------------------------
                    DrawColorBar(row.ConstantItem(15, Unit.Millimetre));
                    row.RelativeItem();
                });

                // On ajuste le padding gauche du contenu
                page.Content().PaddingLeft(3f, Unit.Centimetre).PaddingRight(2, Unit.Centimetre).PaddingVertical(1.5f, Unit.Centimetre).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        if (logoBytes.Length > 0) row.ConstantItem(80).Image(logoBytes);
                        row.RelativeItem().PaddingLeft(10).Column(c =>
                        {
                            c.Item().Text("Agence de Régulation des Postes et des Communications Électroniques").FontSize(10).FontColor("#4E9741");
                            c.Item().PaddingTop(10).Text(referenceNumber).FontSize(10).Bold();
                        });
                        row.ConstantItem(100).AlignRight().Text("COPIE").SemiBold().FontColor(Colors.Grey.Medium);
                    });

                    col.Item().PaddingTop(20).AlignRight().Column(dest =>
                    {
                        dest.Item().Text($"Pointe-Noire, le {DateTime.Now:dd MMMM yyyy}");
                        dest.Item().Height(20);
                        dest.Item().Text("Monsieur le Directeur Général").Bold();
                        dest.Item().Text($"de la Société {dossier.Client?.RaisonSociale}").Bold();
                        dest.Item().Text(dossier.Client?.Ville ?? "POINTE-NOIRE").Bold().Underline();
                    });

                    col.Item().PaddingTop(25).Text("Objet : Homologation des équipements").Bold().Underline();
                    col.Item().Height(20);

                    col.Item().Text("Monsieur le Directeur Général,");
                    col.Item().Height(10);
                    col.Item().Text("J'accuse réception de votre correspondance du [Date Correspondance], par laquelle vous sollicitez l'homologation de deux (02) types d'équipements dans le cadre de vos activités en République du Congo, et je vous en remercie.");
                    col.Item().Height(10);
                    col.Item().Text("A cet effet, je vous informe que l'un de ces types d'équipements n'est pas soumis à homologation. Il s'agit de :");

                    col.Item().PaddingVertical(10).PaddingLeft(20).Row(row =>
                    {
                        row.ConstantItem(10).Text("▪");
                        row.RelativeItem().Text($"{demande.Equipement}, Modèle : {demande.Modele}, Marque : {demande.Marque}").Bold();
                    });

                    col.Item().Text("En conséquence, l'Autorité de régulation vous autorise à importer en République du Congo le type d'équipement susvisé.");
                    col.Item().Height(20);
                    col.Item().Text("Veuillez agréer, Monsieur le Directeur Général, mes salutations distinguées.");

                    col.Item().ExtendVertical().AlignBottom().AlignRight().Column(sig =>
                    {
                        sig.Item().Text("Le Chef d'Antenne Départementale").Bold();
                        sig.Item().PaddingTop(60).Text(_settings.AntennaChiefName).Bold();
                    });
                });

                page.Footer().PaddingHorizontal(2.5f, Unit.Centimetre).PaddingBottom(1, Unit.Centimetre).AlignCenter().Column(c =>
                {
                    c.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    c.Item().PaddingTop(5).Text("Etablissement public administratif doté de la personnalité juridique et de l'autonomie financière placé").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"sous la tutelle du MPTE | Adresse : ... | BP: {_settings.AntennaPOBox} | Email: {_settings.ArpceEmail}").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }
}