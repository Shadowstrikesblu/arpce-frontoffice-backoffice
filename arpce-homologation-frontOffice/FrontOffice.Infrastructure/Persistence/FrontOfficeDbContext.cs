using FrontOffice.Application.Common.Interfaces; 
using FrontOffice.Domain.Common;
using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace FrontOffice.Infrastructure.Persistence;

public class FrontOfficeDbContext : DbContext, IApplicationDbContext
{
    // Injection du service pour obtenir l'utilisateur connecté
    private readonly ICurrentUserService _currentUserService; 

    public FrontOfficeDbContext(
        DbContextOptions<FrontOfficeDbContext> options,
        ICurrentUserService currentUserService 
        ) : base(options)
    {
        _currentUserService = currentUserService; 
    }

    // --- Déclaration de toutes les tables de la base de données ---
<<<<<<< HEAD
    public DbSet<AdminAccess> AdminAccesses => Set<AdminAccess>();
    public DbSet<AdminConnexions> AdminConnexions => Set<AdminConnexions>(); 
    public DbSet<AdminEvenementsTypes> AdminEvenementsTypes => Set<AdminEvenementsTypes>();
    public DbSet<AdminJournal> AdminJournals => Set<AdminJournal>();
    public DbSet<AdminOptions> AdminOptions => Set<AdminOptions>(); 
    public DbSet<AdminProfils> AdminProfils => Set<AdminProfils>();
    public DbSet<AdminProfilsAcces> AdminProfilsAcces => Set<AdminProfilsAcces>();
    public DbSet<AdminProfilsUtilisateursLDAP> AdminProfilsUtilisateursLDAP => Set<AdminProfilsUtilisateursLDAP>();
    public DbSet<AdminReporting> AdminReporting => Set<AdminReporting>();
    public DbSet<AdminUtilisateurs> AdminUtilisateurs => Set<AdminUtilisateurs>();
    public DbSet<AdminUtilisateurTypes> AdminUtilisateurTypes => Set<AdminUtilisateurTypes>();
    public DbSet<Attestation> Attestations => Set<Attestation>();
    public DbSet<CategorieEquipement> CategoriesEquipements => Set<CategorieEquipement>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Commentaire> Commentaires => Set<Commentaire>();
    public DbSet<Demande> Demandes => Set<Demande>();
    public DbSet<Devis> Devis => Set<Devis>();
    public DbSet<DocumentDemande> DocumentsDemandes => Set<DocumentDemande>();
    public DbSet<DocumentDossier> DocumentsDossiers => Set<DocumentDossier>();
    public DbSet<Dossier> Dossiers => Set<Dossier>();
    public DbSet<ModeReglement> ModesReglements => Set<ModeReglement>();
    public DbSet<MotifRejet> MotifsRejets => Set<MotifRejet>();
    public DbSet<Proposition> Propositions => Set<Proposition>();
    public DbSet<Statut> Statuts => Set<Statut>();
=======

    public DbSet<AdminAccess> AdminAccesses { get; set; }
    public DbSet<AdminConnexions> AdminConnexions { get; set; } 
    public DbSet<AdminEvenementsTypes> AdminEvenementsTypes { get; set; }
    public DbSet<AdminJournal> AdminJournals { get; set; }
    public DbSet<AdminOptions> AdminOptions { get; set; } 
    public DbSet<AdminProfils> AdminProfils { get; set; }
    public DbSet<AdminProfilsAcces> AdminProfilsAcces { get; set; }
    public DbSet<AdminProfilsUtilisateursLDAP> AdminProfilsUtilisateursLDAP { get; set; }
    public DbSet<AdminReporting> AdminReporting { get; set; }
    public DbSet<AdminUtilisateur> AdminUtilisateurs { get; set; }
    public DbSet<AdminUtilisateurTypes> AdminUtilisateurTypes { get; set; }
    public DbSet<Attestation> Attestations { get; set; }
    public DbSet<CategorieEquipement> CategoriesEquipements { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Commentaire> Commentaires { get; set; }
    public DbSet<Demande> Demandes { get; set; }
    public DbSet<Devis> Devis { get; set; }
    public DbSet<DocumentDemande> DocumentsDemandes { get; set; }
    public DbSet<DocumentDossier> DocumentsDossiers { get; set; }
    public DbSet<Dossier> Dossiers { get; set; }
    public DbSet<ModeReglement> ModesReglements { get; set; }
    public DbSet<MotifRejet> MotifsRejets { get; set; }
    public DbSet<Proposition> Propositions { get; set; }
    public DbSet<Statut> Statuts { get; set; }
>>>>>>> 7a55aea422e4438075a73bade99823b32b369e91


    /// <summary>
    /// Méthode pour gérer automatiquement les champs d'audit.
    /// Elle est appelée à chaque fois que _context.SaveChangesAsync() est executé.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Récupération de l'ID de l'utilisateur connecté
        var currentUserId = _currentUserService.UserId?.ToString(); 

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.UtilisateurCreation = currentUserId;
                    entry.Entity.DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    break;

                case EntityState.Modified:
                    entry.Entity.UtilisateurModification = currentUserId;
                    entry.Entity.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
