using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

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
    public string? Adresse { get; set; }
    public string? Bp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public string? Remarques { get; set; }
    public string TypeClient { get; set; } = "Entreprise";
    public int NiveauValidation { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    public string? VerificationCode { get; set; }
    public long? VerificationTokenExpiry { get; set; }

    public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();
}