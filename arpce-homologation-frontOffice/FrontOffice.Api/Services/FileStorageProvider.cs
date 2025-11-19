using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace FrontOffice.Api.Services;

/// <summary>
/// Implémentation du service de stockage de fichiers sur le disque local du serveur web.
/// Les fichiers sont stockés dans le dossier wwwroot pour être accessibles publiquement.
/// </summary>
public class LocalFileStorageProvider : IFileStorageProvider
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string UploadsDirectoryName = "uploads";

    public LocalFileStorageProvider(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Sauvegarde un fichier physiquement sur le disque du serveur.
    /// </summary>
    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string subfolder)
    {
        // Construit le chemin complet du dossier de destination dans wwwroot (ex: C:\path\to\project\wwwroot\uploads\courriers)
        var destinationFolder = Path.Combine(_webHostEnvironment.WebRootPath, UploadsDirectoryName, subfolder);

        // S'assure que le dossier de destination existe, sinon le crée.
        Directory.CreateDirectory(destinationFolder);

        // Construit le chemin complet du fichier à créer (ex: C:\path\to\project\wwwroot\uploads\courriers\guid_monfichier.pdf)
        var filePath = Path.Combine(destinationFolder, fileName);

        // Copie le contenu du flux uploadé vers un nouveau fichier sur le disque.
        using (var destinationStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(destinationStream);
        }

        // Retourne le chemin relatif accessible via une URL.
        var relativePath = Path.Combine("/", UploadsDirectoryName, subfolder, fileName).Replace('\\', '/');

        return relativePath;
    }
}