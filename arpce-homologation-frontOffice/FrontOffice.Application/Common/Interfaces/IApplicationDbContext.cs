using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FrontOffice.Application.Common.Interfaces;

public interface IApplicationDbContext
{

    public DbSet<AdminAccess> AdminAccesses { get; }
    public DbSet<AdminConnexions> AdminConnexions { get; } 
    public DbSet<AdminEvenementsTypes> AdminEvenementsTypes { get; }
    public DbSet<AdminJournal> AdminJournals { get; }
    public DbSet<AdminOptions> AdminOptions { get; } 
    public DbSet<AdminProfils> AdminProfils { get; }
    public DbSet<AdminProfilsAcces> AdminProfilsAcces { get; }
    public DbSet<AdminProfilsUtilisateursLDAP> AdminProfilsUtilisateursLDAP { get; }
   public DbSet<AdminReporting> AdminReporting { get; }
    public DbSet<AdminUtilisateur> AdminUtilisateurs { get; }
    public DbSet<AdminUtilisateurTypes> AdminUtilisateurTypes { get; }
    DbSet<Attestation> Attestations { get; }
    DbSet<CategorieEquipement> CategoriesEquipements { get; }
    DbSet<Client> Clients { get; }
    DbSet<Commentaire> Commentaires { get; }
    DbSet<Demande> Demandes { get; }
    DbSet<Devis> Devis { get; }
    DbSet<DocumentDemande> DocumentsDemandes { get; }
    DbSet<DocumentDossier> DocumentsDossiers { get; }
    DbSet<Dossier> Dossiers { get; }
    DbSet<ModeReglement> ModesReglements { get; }
    DbSet<MotifRejet> MotifsRejets { get; }
    DbSet<Proposition> Propositions { get; }
    DbSet<Statut> Statuts { get; }
    DbSet<Notification> Notifications { get; }
    DatabaseFacade Database { get; }
    DbSet<Signataire> Signataires { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}