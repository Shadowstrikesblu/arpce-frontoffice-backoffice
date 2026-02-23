using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Infrastructure.Services;

public class DatabaseFileStorageProvider : IFileStorageProvider
{
    private readonly IApplicationDbContext _context;

    public DatabaseFileStorageProvider(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Importe un document lié à un dossier (ex: Lettre de demande, Facture).
    /// </summary>
    public async Task<DocumentDossier> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId, string? libelle = null)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("Le fichier est vide.");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var document = new DocumentDossier
        {
            Id = Guid.NewGuid(),
            IdDossier = dossierId,
            Nom = nom,
            Libelle = libelle, 
            Extension = Path.GetExtension(file.FileName).TrimStart('.').ToLower(),
            Type = type,
            Donnees = ms.ToArray(),
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_UPLOAD"
        };

        _context.DocumentsDossiers.Add(document);

        return document;
    }

    /// <summary>
    /// Importe un document lié à une demande/équipement (ex: Fiche technique, Annexes).
    /// </summary>
    public async Task<DocumentDemande> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId, string? libelle = null)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("Le fichier est vide.");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var document = new DocumentDemande
        {
            Id = Guid.NewGuid(),
            IdDemande = demandeId,
            Nom = nom,
            Libelle = libelle, 
            Extension = Path.GetExtension(file.FileName).TrimStart('.').ToLower(),
            Donnees = ms.ToArray(),
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_UPLOAD"
        };

        _context.DocumentsDemandes.Add(document);

        return document;
    }

    /// <summary>
    /// Sauvegarde un document généré par le système (ex: Reçu de caisse) à partir d'un tableau d'octets.
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
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_GEN"
        };

        _context.DocumentsDossiers.Add(document);
        return document;
    }

    /// <summary>
    /// Enregistre une image de signature sur le disque et retourne le chemin relatif.
    /// </summary>
    public async Task<string> UploadSignatureAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return string.Empty;

        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "signatures");
        if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/signatures/{fileName}";
    }

    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDossierAsync(Guid documentId)
    {
        var doc = await _context.DocumentsDossiers.FirstOrDefaultAsync(x => x.Id == documentId);
        if (doc == null || doc.Donnees == null) throw new FileNotFoundException();

        return (doc.Donnees, doc.Nom, GetContentType(doc.Extension));
    }

    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDemandeAsync(Guid documentId)
    {
        var doc = await _context.DocumentsDemandes.FirstOrDefaultAsync(x => x.Id == documentId);
        if (doc == null || doc.Donnees == null) throw new FileNotFoundException();

        return (doc.Donnees, doc.Nom, GetContentType(doc.Extension));
    }

    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            "pdf" => "application/pdf",
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}