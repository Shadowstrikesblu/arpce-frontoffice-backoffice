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
                    entry.Entity.DateCreation = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UtilisateurModification = currentUserId; 
                    entry.Entity.DateModification = DateTime.UtcNow;
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
