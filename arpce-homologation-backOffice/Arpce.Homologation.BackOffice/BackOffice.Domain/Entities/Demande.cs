using BackOffice.Domain.Common; 
namespace BackOffice.Domain.Entities;
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
    /// <summary>
    /// Prix unitaire de l'homologation pour ce type d'équipement.
    /// Renseigné par l'agent du Back Office lors de l'instruction.
    /// </summary>
    public decimal? PrixUnitaire { get; set; }
    /// <summary>
    /// Pourcentage de remise appliqué sur cet équipement.
    /// Renseigné par l'agent du Back Office.
    /// </summary>
    /// 
    
    /// <summary>
    /// Indique si l'équipement est soumis à homologation.
    /// Par défaut, il est considéré comme homologable (true).
    /// </summary>
    public bool EstHomologable { get; set; } = true;
    public decimal? Remise { get; set; }
    public virtual Dossier Dossier { get; set; } = default!;
    public virtual CategorieEquipement? CategorieEquipement { get; set; }
    public virtual MotifRejet? MotifRejet { get; set; }
    public virtual Proposition? Proposition { get; set; }
    public virtual ICollection<DocumentDemande> DocumentsDemandes { get; set; } = new List<DocumentDemande>();
    public virtual ICollection<Attestation> Attestations { get; set; } = new List<Attestation>();
}