using BackOffice.Domain.Common; 
using System.Collections.Generic;
namespace BackOffice.Domain.Entities;
public class Dossier : AuditableEntity
{
    public Guid IdClient { get; set; }
    public Guid IdStatut { get; set; }
    public Guid IdModeReglement { get; set; }
    public DateTime DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    public Guid? IdAgentInstructeur { get; set; } 

    public virtual Client Client { get; set; } = default!;
    public virtual Statut Statut { get; set; } = default!;
    public virtual ModeReglement ModeReglement { get; set; } = default!;
    public virtual ICollection<Demande> Demandes { get; set; } = new List<Demande>();
    public virtual ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
    public virtual ICollection<DocumentDossier> DocumentsDossiers { get; set; } = new List<DocumentDossier>();
    public virtual ICollection<Devis> Devis { get; set; } = new List<Devis>();
}