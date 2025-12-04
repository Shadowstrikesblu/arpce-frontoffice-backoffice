using Microsoft.AspNetCore.Http;

namespace FrontOffice.Application.Common.Interfaces;

/// <summary>
/// Fournit une abstraction pour le stockage de fichiers.
/// </summary>
public interface IFileStorageProvider
{
    Task<Guid> ImportDocumentDossierAsync(IFormFile file, string nom, byte type, Guid dossierId);
    Task<Guid> ImportDocumentDemandeAsync(IFormFile file, string nom, Guid demandeId);

    Task<(byte[] content, string fileName, string contentType)> ExportDocumentDossierAsync(Guid documentId);
    Task<(byte[] content, string fileName, string contentType)> ExportDocumentDemandeAsync(Guid documentId);
}