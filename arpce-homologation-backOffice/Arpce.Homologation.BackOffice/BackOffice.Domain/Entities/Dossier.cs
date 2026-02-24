using BackOffice.Domain.Common;
using System;
using System.Collections.Generic;

namespace BackOffice.Domain.Entities;

public class Dossier : AuditableEntity
{
    public Guid IdClient { get; set; }
    public Guid IdStatut { get; set; }
    public Guid? IdModeReglement { get; set; }
    public long DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public long? DateEnvoiDevis { get; set; }         
    public long? DateDemandeEchantillon { get; set; } 
    public bool PenaliteAppliquee { get; set; } = false;
    public bool RappelPaiementEnvoye { get; set; } = false;
    public bool RappelEchantillonEnvoye { get; set; } = false;
    public Guid? IdAgentInstructeur { get; set; }

    public virtual Client Client { get; set; } = default!;
    public virtual Statut Statut { get; set; } = default!;
    public virtual ModeReglement? ModeReglement { get; set; }

    public virtual Demande Demande { get; set; } = default!;

    public virtual ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
    public virtual ICollection<DocumentDossier> DocumentsDossiers { get; set; } = new List<DocumentDossier>();
    public virtual ICollection<Devis> Devis { get; set; } = new List<Devis>();
}