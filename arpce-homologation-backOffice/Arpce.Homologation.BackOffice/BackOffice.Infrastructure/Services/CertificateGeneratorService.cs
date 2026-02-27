using BackOffice.Application.Common;
using BackOffice.Application.Common.Exceptions;
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

public class CertificateGeneratorService : ICertificateGeneratorService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CertificateGeneratorService> _logger;
    private readonly CertificateSettings _settings;
    private readonly string _logoPath;
    private readonly string _signatureBasePath;

    public CertificateGeneratorService(
        IApplicationDbContext context,
        ILogger<CertificateGeneratorService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _settings = configuration.GetSection("CertificateSettings").Get<CertificateSettings>() ?? new CertificateSettings();
        _logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo_arpce.png");
        _signatureBasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        QuestPDF.Settings.License = LicenseType.Community;

        // Gestion de la police Poppins
        try
        {
            var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Poppins-Regular.ttf");
            if (File.Exists(fontPath))
            {
                FontManager.RegisterFont(File.OpenRead(fontPath));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "La police Poppins n'a pas pu être chargée. Utilisation d'Arial par défaut.");
        }
    }

    public async Task GenerateAttestationsForDossierAsync(Guid dossierId)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Client)
            .Include(d => d.Demande)
                .ThenInclude(dem => dem.Attestations)
                .ThenInclude(a => a.Signataire) 
                .ThenInclude(s => s.AdminUtilisateur)
            .FirstOrDefaultAsync(d => d.Id == dossierId);

        if (dossier == null || dossier.Demande == null) return;

        var demande = dossier.Demande;
        byte[] logoBytes = File.Exists(_logoPath) ? await File.ReadAllBytesAsync(_logoPath) : Array.Empty<byte>();

        // On récupère l'attestation existante
        var existingAttestation = demande.Attestations.FirstOrDefault();
        string visaFinal = existingAttestation?.VisaReference ?? GenerateVisaNumber();

        var signataireActuel = existingAttestation?.Signataire;

        byte[] pdfBytes = demande.EstHomologable
            ? GenerateCertificatePdf(dossier, demande, visaFinal, logoBytes, signataireActuel)
            : GenerateLetterPdf(dossier, demande, visaFinal, logoBytes, signataireActuel);

        if (existingAttestation != null)
        {
            existingAttestation.Donnees = pdfBytes;
            existingAttestation.VisaReference = visaFinal;
            _context.Attestations.Update(existingAttestation);
        }
        else
        {
            _context.Attestations.Add(new Attestation
            {
                Id = Guid.NewGuid(),
                IdDemande = demande.Id,
                Donnees = pdfBytes,
                Extension = "pdf",
                VisaReference = visaFinal,
                DateDelivrance = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        await _context.SaveChangesAsync(default);
    }

    private string GenerateVisaNumber()
    {
        var anneeStr = DateTime.UtcNow.ToString("yy");
        var suffix = $"/ARPCE-DG/DAJI/DRSCE/{anneeStr}";
        var sequence = _context.Attestations.Count(a => a.VisaReference != null && a.VisaReference.EndsWith(suffix)) + 1;
        return $"N°{sequence:D4}{suffix}";
    }

    /// <summary>
    /// Génère le PDF de la LETTRE DE NON-SOUMISSION À L'HOMOLOGATION (100% conforme)
    /// </summary>
    private byte[] GenerateLetterPdf(Dossier dossier, Demande demande, string referenceNumber, byte[] logoBytes, Signataire? signataire)
    {
        var fontName = "Poppins"; 

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(fontName));

                page.Header().Column(col => {
                    col.Item().Row(row => {
                        row.ConstantItem(70).Image(logoBytes);
                        row.RelativeItem().PaddingLeft(10).PaddingTop(5).Column(c => {
                            var color = "#2E7D6A";
                            c.Item().Text("Agence de Régulation des Postes").FontSize(16).FontColor(color);
                            c.Item().Text("et des Communications Électroniques").FontSize(16).FontColor(color);
                        });
                    });

                    col.Item().PaddingTop(15).Row(row => {
                        row.RelativeItem().Text($"N°{referenceNumber}").FontSize(10);
                        row.ConstantItem(200).AlignRight().Column(c => {
                            c.Item().Text(text => text.Span("COPIE").FontSize(14).Bold().FontColor(Colors.Red.Medium.WithAlpha(0.7f)));
                            c.Item().Text($"Pointe-Noire, le {DateTime.Now:dd MMMM yyyy}").FontSize(10);
                        });
                    });

                    col.Item().PaddingLeft(20).PaddingTop(20).Border(1.5f).BorderColor(Colors.Blue.Medium).Width(150).Padding(5).Column(c => {
                        c.Item().AlignCenter().Text("REÇU LE").Bold();
                        c.Item().AlignCenter().Text(DateTime.Now.AddDays(-1).ToString("dd MMM. yyyy").ToUpper()).FontSize(14).Bold();
                        c.Item().AlignCenter().Text(dossier.Client.RaisonSociale.Split(' ')[0].ToUpper()).Bold();
                    });
                });

                page.Content().PaddingTop(20).Column(col => {
                    col.Item().AlignRight().PaddingRight(20).Column(c => {
                        c.Item().Text("Monsieur le Directeur Général").Bold();
                        c.Item().Text($"de la Société {dossier.Client?.RaisonSociale?.ToUpper()}").Bold();
                        c.Item().Text(dossier.Client?.Ville?.ToUpper() ?? "POINTE-NOIRE").Bold().Underline();
                    });

                    col.Item().PaddingTop(20).Text("Objet : Homologation des équipements").Bold().Underline();
                    col.Item().PaddingTop(15).Text("Monsieur le Directeur Général,");
                    col.Item().PaddingTop(10).Text($"J'accuse réception de votre correspondance du {dossier.DateOuverture.FromUnixTimeMilliseconds():dd MMMM yyyy}, par laquelle vous sollicitez l'homologation de deux (02) types d'équipements dans le cadre de vos activités en République du Congo, et je vous en remercie.");
                    col.Item().PaddingTop(10).Text("A cet effet, je vous informe que l'un de ces types d'équipements n'est pas soumis à homologation. Il s'agit de :");

                    col.Item().PaddingVertical(10).PaddingLeft(40).Text($"▪ ETIQUETTE ELECTRONIQUE {demande.Equipement}, Modèle : {demande.Modele}, Marque : {demande.Marque}").Bold();
                    col.Item().Text("En conséquence, l'Autorité de régulation vous autorise à importer en République du Congo le type d'équipement susvisé.");
                    col.Item().PaddingTop(15).Text("Veuillez agréer, Monsieur le Directeur Général, mes salutations distinguées.");
                });

                page.Footer().AlignRight().PaddingRight(20).Column(sig => {
                    sig.Item().Text("Le Chef d'Antenne Départementale").Bold();
                    DrawSignature(sig, signataire);
                });
            });
        }).GeneratePdf();
    }

    // --- TEMPLATE : CERTIFICAT D'HOMOLOGATION ---
    private byte[] GenerateCertificatePdf(Dossier dossier, Demande demande, string referenceNumber, byte[] logoBytes, Signataire? signataire)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("New Century Schoolbook"));

                page.Background().Row(row => {
                    row.ConstantItem(1, Unit.Centimetre);
                    DrawColorBar(row.ConstantItem(15, Unit.Millimetre));
                    row.RelativeItem();
                });

                page.Content().PaddingLeft(3f, Unit.Centimetre).PaddingRight(2, Unit.Centimetre).PaddingVertical(1.5f, Unit.Centimetre).Column(col => {
                    col.Item().Row(row => {
                        if (logoBytes.Length > 0) row.ConstantItem(80).Image(logoBytes);
                        row.RelativeItem().PaddingLeft(10).Column(c => {
                            c.Item().Text("Agence de Régulation des Postes et des Communications Électroniques").FontSize(10).FontColor("#4E9741");
                            c.Item().AlignCenter().PaddingTop(10).Text("CERTIFICAT D'HOMOLOGATION").FontSize(16).Bold();
                            c.Item().AlignCenter().Text(referenceNumber).FontSize(12).Bold();
                            c.Item().AlignCenter().Text("----------o00o----------").FontSize(10);
                        });
                    });

                    col.Item().PaddingTop(20).Text(_settings.LegalReference).Justify();
                    col.Item().Height(15);
                    col.Item().Table(table => {
                        table.ColumnsDefinition(columns => { columns.ConstantColumn(180); columns.RelativeColumn(); });
                        void AddRow(string label, string? value) { table.Cell().PaddingBottom(2).Text(label).Bold(); table.Cell().PaddingBottom(2).Text($": {value ?? "N/A"}"); }
                        AddRow("Equipement", demande.Equipement);
                        AddRow("Modèle", demande.Modele);
                        AddRow("Marque", demande.Marque);
                        AddRow("Description", demande.Description);
                        AddRow("Fabricant", demande.Fabricant);
                    });

                    col.Item().PaddingTop(10).Text("Documents techniques présentés : Schémas techniques, description technique, rapport de test,").FontSize(11);
                    col.Item().Height(15);
                    col.Item().Text($"Nom de la société (demandeur) : {dossier.Client?.RaisonSociale}").Bold();
                    col.Item().Text($"Adresse : {dossier.Client?.Adresse ?? "N/A"}").Bold();
                    col.Item().Text($"Tél : {dossier.Client?.ContactTelephone}   E-mail : {dossier.Client?.Email}").Bold();
                    col.Item().Height(25);

                    col.Item().ExtendVertical().AlignBottom().AlignRight().Column(sig => {
                        sig.Item().Text($"Fait à Brazzaville, le {DateTime.Now:dd MMMM yyyy}");
                        sig.Item().PaddingTop(20);
                        DrawSignature(sig, signataire);
                    });
                });
            });
        }).GeneratePdf();
    }

    private void DrawSignature(ColumnDescriptor column, Signataire? signataire)
    {
        if (signataire != null && signataire.AdminUtilisateur != null)
        {
            column.Item().Text(signataire.AdminUtilisateur.Fonction ?? "Le Directeur Général").Bold();
            string fullPath = Path.Combine(_signatureBasePath, (signataire.SignatureImagePath ?? "").TrimStart('/', '\\'));
            if (File.Exists(fullPath))
            {
                column.Item().Height(40).Image(File.ReadAllBytes(fullPath)).FitArea();
            }
            else
            {
                column.Item().Height(40);
            }
            column.Item().Text($"{signataire.AdminUtilisateur.Prenoms} {signataire.AdminUtilisateur.Nom}".ToUpper()).Bold();
        }
        else
        {
            column.Item().Text("Le Directeur Général,").Bold();
            column.Item().Height(60);
            column.Item().Text(_settings.DirectorGeneralName).Bold();
        }
    }

    private void DrawColorBar(IContainer container)
    {
        container.Row(row => {
            row.ConstantItem(5, Unit.Millimetre).Background("#009A44");
            row.ConstantItem(5, Unit.Millimetre).Background("#F8CD29");
            row.ConstantItem(5, Unit.Millimetre).Background("#CE2A2D");
        });
    }
}