namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersOverview;

/// <summary>
/// Représente un aperçu statistique des dossiers pour le tableau de bord du Back Office.
/// </summary>
public class DossiersOverviewDto
{
    /// <summary>
    /// Le nombre total de dossiers dans le système.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Le nombre de dossiers ayant un statut de succès (ex: "Approuvé, attestation signée").
    /// </summary>
    public int Success { get; set; }

    /// <summary>
    /// Le nombre de dossiers ayant un statut d'échec ou de rejet.
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Le nombre de dossiers qui sont en cours de traitement.
    /// </summary>
    public int InProgress { get; set; }
}