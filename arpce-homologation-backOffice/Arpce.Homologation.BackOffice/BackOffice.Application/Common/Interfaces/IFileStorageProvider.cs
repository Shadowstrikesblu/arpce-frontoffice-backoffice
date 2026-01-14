using Microsoft.AspNetCore.Http;
using BackOffice.Domain.Entities; 
using System;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.Interfaces;

public interface IFileStorageProvider
{
    /// <summary>
    /// Sauvegarde un fichie en mémoire et le persiste en tant que DocumentDossier via EF Core.
    /// </summary>
    Task<DocumentDossier> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId);

    /// <summary>
    /// Sauvegarde un fichie en mémoire et le persiste en tant que DocumentDemande via EF Core.
    /// </summary>
    Task<DocumentDemande> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId);

    Task<(byte[] content, string fileName, string contentType)> ExportDocumentDossierAsync(Guid documentId);
    Task<(byte[] content, string fileName, string contentType)> ExportDocumentDemandeAsync(Guid documentId);
}