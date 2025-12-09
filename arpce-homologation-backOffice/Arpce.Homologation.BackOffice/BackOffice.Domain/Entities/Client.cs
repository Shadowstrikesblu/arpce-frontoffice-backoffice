using BackOffice.Domain.Common; 

namespace BackOffice.Domain.Entities;

/// <summary>
/// Représente un client (redevable) de la plateforme.
/// Hérite des propriétés d'audit (création, modification).
/// </summary>
public class Client : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string RaisonSociale { get; set; } = string.Empty;
    public string? RegistreCommerce { get; set; }
    public string? MotPasse { get; set; }
    public byte? ChangementMotPasse { get; set; }
    public byte? Desactive { get; set; }

    public string? ContactNom { get; set; }
    public string? ContactTelephone { get; set; }
    public string? ContactFonction { get; set; }

    public string? Email { get; set; }

    // --- NOUVEAUX CHAMPS COMPLETS ---
    public string? Adresse { get; set; }
    public string? Bp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public string? Remarques { get; set; }

    /// <summary>
    /// Type de client : "Particulier" ou "Entreprise".
    /// </summary>
    public string TypeClient { get; set; } = "Entreprise";

    /// <summary>
    /// Niveau de validation du compte.
    /// 0 : Inscrit (en attente OTP)
    /// 1 : OTP Validé (en attente ARPCE)
    /// 2 : Validé ARPCE (Actif)
    /// </summary>
    public int NiveauValidation { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    public string? VerificationCode { get; set; }
    public long? VerificationTokenExpiry { get; set; }

    public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();
}