using FrontOffice.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FrontOffice.Application.Common.Interfaces;

/// <summary>
/// Fournit une abstraction pour le stockage de fichiers.
/// </summary>
public interface IFileStorageProvider
{
    /// <summary>
    /// Sauvegarde un fichier et le persiste avec un libellé optionnel.
    /// </summary>
    Task<DocumentDossier> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId, string? libelle = null);

    /// <summary>
    /// Sauvegarde une fiche technique ou annexe avec un libellé optionnel.
    /// </summary>
    Task<DocumentDemande> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId, string? libelle = null);

    Task<(byte[] content, string fileName, string contentType)> ExportDocumentDossierAsync(Guid documentId);
    Task<(byte[] content, string fileName, string contentType)> ExportDocumentDemandeAsync(Guid documentId);
    Task<string> UploadSignatureAsync(IFormFile file);

    /// <summary>
    /// Sauvegarde un document généré (ex: reçu) avec un libellé.
    /// </summary>
    Task<DocumentDossier> SaveDocumentDossierFromBytesAsync(byte[] content, string nom, byte type, Guid dossierId, string? libelle = null);
}