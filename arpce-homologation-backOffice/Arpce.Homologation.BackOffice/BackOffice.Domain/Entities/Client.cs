using BackOffice.Domain.Common; 

namespace BackOffice.Domain.Entities;

/// <summary>
/// Représente un client (redevable) de la plateforme.
/// Hérite des propriétés d'audit (création, modification).
/// </summary>
public class Client : AuditableEntity
{
    /// <summary>
    /// Code unique attribué au client.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Raison sociale ou nom de l'entreprise du client.
    /// </summary>
    public string RaisonSociale { get; set; } = string.Empty;

    /// <summary>
    /// Numéro du registre de commerce du client (optionnel).
    /// </summary>
    public string? RegistreCommerce { get; set; }

    /// <summary>
    /// Hash du mot de passe du client.
    /// </summary>
    public string? MotPasse { get; set; }

    /// <summary>
    /// Indique si le client doit changer son mot de passe à la prochaine connexion (0=non, 1=oui).
    /// </summary>
    public byte? ChangementMotPasse { get; set; } 

    /// <summary>
    /// Indique si le compte client est désactivé (0=actif, 1=désactivé).
    /// </summary>
    public byte? Desactive { get; set; } 

    /// <summary>
    /// Nom complet de la personne contact principale chez le client.
    /// </summary>
    public string? ContactNom { get; set; }

    /// <summary>
    /// Numéro de téléphone de la personne contact principale.
    /// </summary>
    public string? ContactTelephone { get; set; }

    /// <summary>
    /// Fonction de la personne contact principale.
    /// </summary>
    public string? ContactFonction { get; set; }

    /// <summary>
    /// Adresse e-mail du client.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Adresse physique du client.
    /// </summary>
    public string? Adresse { get; set; }

    /// <summary>
    /// Boîte Postale du client.
    /// </summary>
    public string? Bp { get; set; }

    /// <summary>
    /// Ville du client.
    /// </summary>
    public string? Ville { get; set; }

    /// <summary>
    /// Pays du client.
    /// </summary>
    public string? Pays { get; set; }

    /// <summary>
    /// Remarques diverses concernant le client.
    /// </summary>
    public string? Remarques { get; set; }
    /// <summary>
    /// Indique si le compte du client a été vérifié par e-mail.
    /// Par défaut, un nouveau compte n'est pas vérifié.
    /// </summary>
    public bool IsVerified { get; set; } = false;
    /// <summary>
    /// Le code de vérification à 6 chiffres envoyé par e-mail.
    /// </summary>
    public string? VerificationCode { get; set; }
    /// <summary>
    /// La date d'expiration du code/token de vérification.
    /// </summary>
    public DateTime? VerificationTokenExpiry { get; set; }

    /// <summary>
    /// Collection des dossiers d'homologation soumis par ce client.
    /// </summary>
    public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();
}