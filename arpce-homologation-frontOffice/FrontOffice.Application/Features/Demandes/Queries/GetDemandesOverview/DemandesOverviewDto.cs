namespace FrontOffice.Application.Features.Demandes.Queries.GetDemandesOverview;

/// <summary>
/// Représente le DTO de l'aperçu des demandes pour le tableau de bord.
/// </summary>
public class DemandesOverviewDto
{
    /// <summary>
    /// Le nombre total de demandes du client.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Le nombre de demandes validées.
    /// </summary>
    public int Success { get; set; }

    /// <summary>
    /// Le nombre de demandes rejetées.
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Le nombre de demandes en cours de traitement.
    /// </summary>
    public int InProgress { get; set; }

    /// <summary>
    /// Le nombre de demandes en attente de paiement.
    /// </summary>
    public int PendingPayments { get; set; }
}