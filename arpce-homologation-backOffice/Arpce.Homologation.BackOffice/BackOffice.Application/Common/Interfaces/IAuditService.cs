namespace BackOffice.Application.Common.Interfaces;

/// <summary>
/// Définit un contrat pour le service de journalisation des actions métier (audit).
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Enregistre une action métier dans le journal d'audit.
    /// </summary>
    /// <param name="page">La fonctionnalité ou la page concernée (ex: "Gestion des Profils").</param>
    /// <param name="libelle">Description de l'action effectuée (ex: "Création du profil AGENT_TEST").</param>
    /// <param name="eventTypeCode">Code du type d'événement (ex: "CREATION", "MODIFICATION", "SUPPRESSION").</param>
    /// <param name="dossierId">ID du dossier concerné, si applicable.</param>
    Task LogAsync(string page, string libelle, string eventTypeCode = "MODIFICATION", Guid? dossierId = null);
}