using FrontOffice.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontOffice.Domain.Entities;

public class Demande : AuditableEntity
{
    public Guid IdDossier { get; set; }
    public Guid? IdCategorie { get; set; }
    public Guid? IdMotifRejet { get; set; }
    public Guid? IdProposition { get; set; }
    public string? NumeroDemande { get; set; }
    public string? Equipement { get; set; }
    public string? Modele { get; set; }
    public string? Marque { get; set; }
    public string? Fabricant { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public int? QuantiteEquipements { get; set; }
    public string? ContactNom { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactTelephone { get; set; }

    public Guid? IdStatut { get; set; }
    [ForeignKey("IdStatut")]
    public virtual Statut? Statut { get; set; }

    public decimal? PrixUnitaire { get; set; }
    public decimal? Remise { get; set; }
    public bool EstHomologable { get; set; } = true;

    // Logique d'échantillonnage
    public bool RequiertEchantillon { get; set; }
    public bool EchantillonSoumis { get; set; }

    // Relation 1:1 avec le bénéficiaire
    public virtual Beneficiaire? Beneficiaire { get; set; }

    // Relations
    public virtual Dossier Dossier { get; set; } = default!;
    public virtual CategorieEquipement? CategorieEquipement { get; set; }
    public virtual MotifRejet? MotifRejet { get; set; }
    public virtual Proposition? Proposition { get; set; }

    public virtual ICollection<Devis> Devis { get; set; } = new List<Devis>();
    public virtual ICollection<DocumentDemande> DocumentsDemandes { get; set; } = new List<DocumentDemande>();
    public virtual ICollection<Attestation> Attestations { get; set; } = new List<Attestation>();
}