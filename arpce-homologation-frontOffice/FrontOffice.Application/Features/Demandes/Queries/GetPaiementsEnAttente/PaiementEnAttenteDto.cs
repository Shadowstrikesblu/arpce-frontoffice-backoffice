namespace FrontOffice.Application.Features.Demandes.Queries.GetPaiementsEnAttente;

/// <summary>
/// Représente un paiement en attente, incluant les détails du dossier et du mode de paiement.
/// </summary>
public class PaiementEnAttenteDto
{
    /// <summary>
    /// Identifiant unique du dossier.
    /// </summary>
    public Guid Id { get; set; } 

    /// <summary>
    /// Numéro formaté du dossier (HOM-YYYY-NNN).
    /// </summary>
    public string NumeroDemande { get; set; } = string.Empty;

    /// <summary>
    /// Montant dû pour le paiement.
    /// </summary>
    public decimal Montant { get; set; } 

    /// <summary>
    /// Date limite pour effectuer le paiement.
    /// </summary>
    public DateTime DateEcheance { get; set; } 
    public string NumeroDossier { get; set; }= string.Empty;
    /// <summary>
    /// Mode de règlement choisi pour ce dossier.
    /// </summary>
    //public string ModeReglementLibelle { get; set; } = string.Empty; 
}