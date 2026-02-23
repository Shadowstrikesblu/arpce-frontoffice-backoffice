using Microsoft.AspNetCore.Http;
using BackOffice.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.Interfaces;

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