using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

public class Dossier : AuditableEntity
{
    public Guid IdClient { get; set; }
    public Guid IdStatut { get; set; }
    public Guid IdModeReglement { get; set; }
    public DateTime DateOuverture { get; set; }
    public string Numero { get; set; }
    public string Libelle { get; set; }

    public virtual Client Client { get; set; }
    public virtual Statut Statut { get; set; }
    public virtual ModeReglement ModeReglement { get; set; }
    public virtual ICollection<Demande> Demandes { get; set; } = new List<Demande>();
    public virtual ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
    public virtual ICollection<DocumentDossier> DocumentsDossiers { get; set; } = new List<DocumentDossier>();
    public virtual ICollection<Devis> Devis { get; set; } = new List<Devis>();
}