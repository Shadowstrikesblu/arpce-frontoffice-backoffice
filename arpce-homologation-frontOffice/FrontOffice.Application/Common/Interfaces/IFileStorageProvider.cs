namespace FrontOffice.Application.Common.Interfaces;

/// <summary>
/// Fournit une abstraction pour le stockage de fichiers.
/// </summary>
public interface IFileStorageProvider
{
    /// <summary>
    /// Sauvegarde un fichier dans un sous-dossier spécifié et retourne son chemin relatif.
    /// </summary>
    /// <param name="fileStream">Le flux de données du fichier à sauvegarder.</param>
    /// <param name="fileName">Le nom du fichier à créer.</param>
    /// <param name="subfolder">Le sous-dossier de destination (ex: "courriers", "fiches-techniques").</param>
    /// <returns>Le chemin relatif du fichier sauvegardé (ex: "/uploads/courriers/monfichier.pdf").</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string subfolder);
}