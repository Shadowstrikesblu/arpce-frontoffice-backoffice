namespace BackOffice.Application.Common.Interfaces;

/// <summary>
/// Service responsable de la génération automatique des certificats d'homologation.
/// </summary>
public interface ICertificateGeneratorService
{
    /// <summary>
    /// Génère (ou régénère) les attestations pour tous les équipements d'un dossier donné.
    /// </summary>
    /// <param name="dossierId">L'identifiant du dossier payé.</param>
    Task GenerateAttestationsForDossierAsync(Guid dossierId);
}