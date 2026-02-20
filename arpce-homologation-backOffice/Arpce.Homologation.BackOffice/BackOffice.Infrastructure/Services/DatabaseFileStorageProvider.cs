using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

/// <summary>
/// Gère le stockage des fichiers directement dans la base de données via Entity Framework Core.
/// </summary>
/// <summary>
/// Gère le stockage des fichiers directement dans la base de données via Entity Framework Core.
/// Cette version est modifiée pour ne pas sauvegarder automatiquement, permettant le contrôle par une transaction externe.
/// </summary>
public class DatabaseFileStorageProvider : IFileStorageProvider
{
    private readonly IApplicationDbContext _context;

    public DatabaseFileStorageProvider(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Prépare une entité DocumentDossier pour l'insertion sans la sauvegarder.
    /// </summary>
    public async Task<DocumentDossier> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId)
    {
        // Lit le contenu du fichier en mémoire
        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileData = memoryStream.ToArray();
        }

        // Crée l'entité DocumentDossier
        var document = new DocumentDossier
        {
            Id = Guid.NewGuid(),
            IdDossier = dossierId,
            Nom = nom,
            Type = type,
            Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? string.Empty,
            Donnees = fileData,
            UtilisateurCreation = "API_UPLOAD",
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Ajoute l'entité au contexte
        _context.DocumentsDossiers.Add(document);

        return document;
    }

    /// <summary>
    /// Prépare une entité DocumentDemande pour l'insertion sans la sauvegarder.
    /// </summary>
    public async Task<DocumentDemande> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId)
    {
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
            Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? string.Empty,
            Donnees = fileData,
            UtilisateurCreation = "API_UPLOAD",
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.DocumentsDemandes.Add(document);

        return document;
    }

    /// <summary>
    /// Récupère les données binaires d'un DocumentDossier depuis la base.
    /// </summary>
    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDossierAsync(Guid documentId)
    {
        var document = await _context.DocumentsDossiers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null || document.Donnees == null || document.Donnees.Length == 0)
        {
            throw new FileNotFoundException("Document de dossier introuvable ou vide.");
        }

        var fileName = $"{document.Nom}.{document.Extension}";
        var contentType = GetMimeType(fileName);

        return (document.Donnees, fileName, contentType);
    }

    /// <summary>
    /// Récupère les données binaires d'un DocumentDemande depuis la base.
    /// </summary>
    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDemandeAsync(Guid documentId)
    {
        var document = await _context.DocumentsDemandes
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null || document.Donnees == null || document.Donnees.Length == 0)
        {
            throw new FileNotFoundException("Document de demande introuvable ou vide.");
        }

        var fileName = $"{document.Nom}.{document.Extension}";
        var contentType = GetMimeType(fileName);

        return (document.Donnees, fileName, contentType);
    }

    private string GetMimeType(string fileName)
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

    public async Task<string> UploadSignatureAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return string.Empty;

        // On définit le dossier de stockage (ex: wwwroot/uploads/signatures)
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "signatures");

        if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // On retourne l'URL relative pour le Front-end
        return $"/uploads/signatures/{fileName}";
    }
}