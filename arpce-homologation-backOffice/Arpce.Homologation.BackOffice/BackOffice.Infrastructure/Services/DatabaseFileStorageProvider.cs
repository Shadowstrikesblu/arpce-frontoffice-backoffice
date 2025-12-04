using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services;

public class DatabaseFileStorageProvider : IFileStorageProvider
{
    private readonly IApplicationDbContext _context;
    private readonly string _tempUploadPath; // Chemin local temporaire sur le serveur

    public DatabaseFileStorageProvider(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        // Lire le chemin du dossier temporaire depuis la configuration
        _tempUploadPath = configuration["FileStorageSettings:TempUploadPath"] ?? Path.Combine(Path.GetTempPath(), "arpce_uploads");
        Directory.CreateDirectory(_tempUploadPath);
    }

    /// <summary>
    /// Importe un fichier pour un DocumentDossier.
    /// </summary>
    public async Task<Guid> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId)
    {
        var tempFilePath = await SaveFileToTempLocation(file);

        try
        {
            // Paramètres pour la procédure stockée ImporterDocumentDossier
            var fileParam = new SqlParameter("@fichier", tempFilePath);
            var nomParam = new SqlParameter("@nom", nom);
            var typeParam = new SqlParameter("@type", type);
            var dossierIdParam = new SqlParameter("@IDDossier", dossierId);

            // Paramètres de sortie
            var idDocumentParam = new SqlParameter("@IDDocument", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output };
            var errNumParam = new SqlParameter("@errNumero", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var errMsgParam = new SqlParameter("@errMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC ImporterDocumentDossier @fichier, @nom, @type, @IDDossier, @IDDocument OUT, @errNumero OUT, @errMessage OUT",
                fileParam, nomParam, typeParam, dossierIdParam, idDocumentParam, errNumParam, errMsgParam
            );

            // Gérer les erreurs retournées par la procédure stockée
            if ((int)errNumParam.Value != 0)
            {
                throw new Exception($"Erreur SQL lors de l'import: {(string)errMsgParam.Value} (Code: {(int)errNumParam.Value})");
            }

            // Retourner l'ID du document créé
            return (Guid)idDocumentParam.Value;
        }
        finally
        {
            // Nettoyer le fichier temporaire
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    /// <summary>
    /// Importe un fichier pour un DocumentDemande.
    /// </summary>
    public async Task<Guid> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId)
    {
        var tempFilePath = await SaveFileToTempLocation(file);

        try
        {
            var fileParam = new SqlParameter("@fichier", tempFilePath);
            var nomParam = new SqlParameter("@nom", nom);
            var demandeIdParam = new SqlParameter("@IDDemande", demandeId);

            var idDocumentParam = new SqlParameter("@IDDocument", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output };
            var errNumParam = new SqlParameter("@errNumero", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var errMsgParam = new SqlParameter("@errMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC ImporterDocumentDemande @fichier, @nom, @IDDemande, @IDDocument OUT, @errNumero OUT, @errMessage OUT",
                fileParam, nomParam, demandeIdParam, idDocumentParam, errNumParam, errMsgParam
            );

            if ((int)errNumParam.Value != 0)
            {
                throw new Exception($"Erreur SQL lors de l'import: {(string)errMsgParam.Value} (Code: {(int)errNumParam.Value})");
            }

            return (Guid)idDocumentParam.Value;
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    /// <summary>
    /// Exporte un DocumentDossier en utilisant la procédure stockée.
    /// </summary>
    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDossierAsync(Guid documentId)
    {
        // ... Logique d'exportation similaire à l'importation ...
        // Le script fourni est problématique pour une API car il écrit sur un disque serveur.
        // Une meilleure approche serait de lire directement les `Donnees` de la table.
        // Je vais implémenter cette meilleure approche.

        var document = await _context.DocumentsDossiers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null || document.Donnees == null)
        {
            throw new FileNotFoundException("Document introuvable ou vide.");
        }

        var fileName = $"{document.Nom}.{document.Extension}";
        var contentType = GetMimeType(fileName); // Méthode utilitaire à créer

        return (document.Donnees, fileName, contentType);
    }

    // Logique d'exportation pour DocumentDemande (identique)
    public async Task<(byte[] content, string fileName, string contentType)> ExportDocumentDemandeAsync(Guid documentId)
    {
        var document = await _context.DocumentsDemandes
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null || document.Donnees == null)
        {
            throw new FileNotFoundException("Document introuvable ou vide.");
        }

        var fileName = $"{document.Nom}.{document.Extension}";
        var contentType = GetMimeType(fileName);

        return (document.Donnees, fileName, contentType);
    }


    // --- Méthodes privées utilitaires ---

    private async Task<string> SaveFileToTempLocation(IFormFile file)
    {
        // Crée un nom de fichier unique et le sauvegarde dans le dossier temporaire
        var tempFilePath = Path.Combine(_tempUploadPath, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return tempFilePath;
    }

    private string GetMimeType(string fileName)
    {
        // Logique simple pour déterminer le content type
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