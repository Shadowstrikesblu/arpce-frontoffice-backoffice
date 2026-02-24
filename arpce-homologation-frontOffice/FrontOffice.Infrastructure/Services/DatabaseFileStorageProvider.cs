using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FrontOffice.Infrastructure.Services;

/// <summary>
/// Gère le stockage des fichiers directement dans la base de données via Entity Framework Core.
/// Cette version implémente l'interface complète incluant les libellés et la gestion des flux binaires.
/// </summary>
public class DatabaseFileStorageProvider : IFileStorageProvider
{
    private const string DefaultUser = "API_UPLOAD";
    private const byte SignatureDocType = 99;

    private readonly IApplicationDbContext _context;

    public DatabaseFileStorageProvider(IApplicationDbContext context)
    {
        _context = context;
    }

    // --------------------------------------------------------------------
    // REQUIRED BY IFileStorageProvider (incoming changes)
    // --------------------------------------------------------------------

    /// <summary>
    /// Prépare une entité DocumentDossier pour l'insertion (avec libellé optionnel).
    /// </summary>
    public async Task<DocumentDossier> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId, string? libelle = null)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("Le fichier est vide.");

        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileData = memoryStream.ToArray();
        }

        var document = new DocumentDossier
        {
            Id = Guid.NewGuid(),
            IdDossier = dossierId,
            Nom = nom,
            Libelle = libelle, 
            Type = type,
            Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? string.Empty,
            Donnees = fileData,
            UtilisateurCreation = "API_UPLOAD",
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDossiers.Add(document);
        return document;
    }

    /// <summary>
    /// Prépare une entité DocumentDemande pour l'insertion (avec libellé optionnel).
    /// </summary>
    public async Task<DocumentDemande> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId, string? libelle = null)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("Le fichier est vide.");

        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileData = memoryStream.ToArray();
        }

        var document = new DocumentDemande
        {
            Id = Guid.NewGuid(),
            IdDemande = demandeId,
            Nom = nom,
            Libelle = libelle, // Prise en compte du libellé
            Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? string.Empty,
            Donnees = fileData,
            UtilisateurCreation = string.IsNullOrWhiteSpace(utilisateurCreation) ? DefaultUser : utilisateurCreation,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDemandes.Add(document);
        return document;
    }

    /// <summary>
    /// Sauvegarde un document généré (ex: reçu PDF) directement à partir de données binaires.
    /// </summary>
    public async Task<DocumentDossier> SaveDocumentDossierFromBytesAsync(byte[] content, string nom, byte type, Guid dossierId, string? libelle = null)
    {
        var document = new DocumentDossier
        {
            Id = Guid.NewGuid(),
            IdDossier = dossierId,
            Nom = nom,
            Libelle = libelle,
            Extension = "pdf",
            Type = type,
            Donnees = content,
            UtilisateurCreation = "SYSTEM_GEN",
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDossiers.Add(document);

        return document;
    }

    /// <summary>
    /// Signature factice pour le FrontOffice (le stockage physique des signatures est géré par le BackOffice).
    /// Implémenté pour respecter le contrat d'interface.
    /// </summary>
    public Task<string> UploadSignatureAsync(IFormFile file)
    {
        // Le Front-Office n'a généralement pas besoin de cette méthode
        throw new NotImplementedException("La gestion des images de signature est réservée au Back-Office.");
    }

    /// <summary>
    /// Récupère les données binaires d'un DocumentDossier.
    /// </summary>
    public Task<DocumentDossier> SaveDocumentDossierFromBytesAsync(
        byte[] data,
        string nom,
        byte type,
        Guid dossierId,
        string? utilisateurCreation)
    {
        if (data == null || data.Length == 0)
            throw new InvalidOperationException("Données vides.");

        var document = new DocumentDossier
        {
            Id = Guid.NewGuid(),
            IdDossier = dossierId,
            Nom = nom,
            Type = type,
            Extension = "pdf", // si vous devez conserver l'extension, ajoutez-la en paramètre
            Donnees = data,
            UtilisateurCreation = string.IsNullOrWhiteSpace(utilisateurCreation) ? DefaultUser : utilisateurCreation,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDossiers.Add(document);
        return Task.FromResult(document);
    }

    // --------------------------------------------------------------------
    // Backward-compatible overloads (your old signatures)
    // --------------------------------------------------------------------

    public Task<DocumentDossier> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId)
        => ImportDocumentDossierAsync(file, nom, type, dossierId, DefaultUser);

    public Task<DocumentDemande> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId)
        => ImportDocumentDemandeAsync(file, nom, demandeId, DefaultUser);

    // --------------------------------------------------------------------
    // Export methods (unchanged)
    // --------------------------------------------------------------------

    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDossierAsync(Guid documentId)
    {
        var document = await _context.DocumentsDossiers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null || document.Donnees == null || document.Donnees.Length == 0)
            throw new FileNotFoundException("Document de dossier introuvable ou vide.");

        var fileName = $"{document.Nom}.{document.Extension}";
        var contentType = GetMimeType(fileName);

        return (document.Donnees, fileName, contentType);
    }

    /// <summary>
    /// Récupère les données binaires d'un DocumentDemande.
    /// </summary>
    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDemandeAsync(Guid documentId)
    {
        var document = await _context.DocumentsDemandes
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null || document.Donnees == null || document.Donnees.Length == 0)
            throw new FileNotFoundException("Document de demande introuvable ou vide.");

        var fileName = $"{document.Nom}.{document.Extension}";
        var contentType = GetMimeType(fileName);

        return (document.Donnees, fileName, contentType);
    }

    // --------------------------------------------------------------------
    // Helpers
    // --------------------------------------------------------------------

    private static async Task<byte[]> ReadFileBytesAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private static string GetExtension(string fileName)
        => Path.GetExtension(fileName)?.TrimStart('.').ToLowerInvariant() ?? string.Empty;

    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream",
        };
    }
}
