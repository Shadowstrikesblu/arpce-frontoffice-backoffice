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
/// Cette version n'appelle PAS SaveChanges, permettant le contrôle par une transaction externe.
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
    /// Prépare une entité DocumentDossier pour l'insertion sans la sauvegarder.
    /// </summary>
    public async Task<DocumentDossier> ImportDocumentDossierAsync(
        IFormFile file,
        string nom,
        byte type,
        Guid dossierId,
        string? utilisateurCreation)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Fichier invalide ou vide.");

        var fileData = await ReadFileBytesAsync(file);

        var document = new DocumentDossier
        {
            Id = Guid.NewGuid(),
            IdDossier = dossierId,
            Nom = nom,
            Type = type,
            Extension = GetExtension(file.FileName),
            Donnees = fileData,
            UtilisateurCreation = string.IsNullOrWhiteSpace(utilisateurCreation) ? DefaultUser : utilisateurCreation,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDossiers.Add(document);
        return document;
    }

    /// <summary>
    /// Prépare une entité DocumentDemande pour l'insertion sans la sauvegarder.
    /// </summary>
    public async Task<DocumentDemande> ImportDocumentDemandeAsync(
        IFormFile file,
        string nom,
        Guid demandeId,
        string? utilisateurCreation)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Fichier invalide ou vide.");

        var fileData = await ReadFileBytesAsync(file);

        var document = new DocumentDemande
        {
            Id = Guid.NewGuid(),
            IdDemande = demandeId,
            Nom = nom,
            Extension = GetExtension(file.FileName),
            Donnees = fileData,
            UtilisateurCreation = string.IsNullOrWhiteSpace(utilisateurCreation) ? DefaultUser : utilisateurCreation,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDemandes.Add(document);
        return document;
    }

    /// <summary>
    /// Enregistre une signature en base (comme DocumentDossier "signature") sans sauvegarder.
    /// Retourne un identifiant (Guid) sous forme de string.
    /// </summary>
    public async Task<string> UploadSignatureAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Signature invalide ou vide.");

        var fileData = await ReadFileBytesAsync(file);
        var id = Guid.NewGuid();

        // Si vous avez une table dédiée aux signatures, remplacez ceci par votre entité.
        var document = new DocumentDossier
        {
            Id = id,
            IdDossier = Guid.Empty,      // pas lié à un dossier ici (adapter si besoin)
            Nom = "signature",
            Type = SignatureDocType,
            Extension = GetExtension(file.FileName),
            Donnees = fileData,
            UtilisateurCreation = DefaultUser,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDossiers.Add(document);

        // Pas de SaveChanges ici (transaction externe)
        return id.ToString();
    }

    /// <summary>
    /// Crée un DocumentDossier à partir d'un byte[] sans sauvegarder.
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
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream",
        };
    }
}
