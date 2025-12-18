using BackOffice.Application.Common.Interfaces; 
using BackOffice.Domain.Common; 
using BackOffice.Domain.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.ChangeTracking; 
using Microsoft.EntityFrameworkCore.Diagnostics; 
using System.Reflection; 

namespace BackOffice.Infrastructure.Persistence;

/// <summary>
/// Contexte de base de données concret pour le microservice Back Office.
/// Il implémente l'interface IApplicationDbContext et hérite de DbContext d'Entity Framework Core.
/// Gère la persistance de toutes les entités du domaine Back Office.
/// </summary>
public class BackOfficeDbContext : DbContext, IApplicationDbContext
{
    // Service injecté pour obtenir l'identifiant de l'utilisateur actuellement connecté.
    // Utilisé pour renseigner automatiquement les champs d'audit (UtilisateurCreation, UtilisateurModification).
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du contexte de base de données du Back Office.
    /// </summary>
    /// <param name="options">Options de configuration du DbContext (par exemple, la chaîne de connexion).</param>
    /// <param name="currentUserService">Service pour récupérer l'utilisateur courant, injecté via DI.</param>
    public BackOfficeDbContext(
        DbContextOptions<BackOfficeDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    // --- Déclaration de tous les ensembles d'entités (DbSet) pour les entités du domaine ---
    // Chaque propriété DbSet représente une table dans la base de données.

    // Entités spécifiques à l'administration du Back Office
    public DbSet<AdminAccess> AdminAccesses { get; set; } = default!;
    public DbSet<AdminConnexions> AdminConnexions { get; set; } = default!;
    public DbSet<AdminEvenementsTypes> AdminEvenementsTypes { get; set; } = default!;
    public DbSet<AdminJournal> AdminJournals { get; set; } = default!;
    public DbSet<AdminOptions> AdminOptions { get; set; } = default!; 
    public DbSet<AdminReporting> AdminReportings { get; set; } = default!;
    public DbSet<AdminUtilisateur> AdminUtilisateurs { get; set; } = default!;
    public DbSet<AdminUtilisateurTypes> AdminUtilisateurTypes { get; set; } = default!;
    public DbSet<AdminProfils> AdminProfils { get; set; } = default!;
    public DbSet<AdminProfilsAcces> AdminProfilsAcces { get; set; } = default!;
    public DbSet<AdminProfilsUtilisateursLDAP> AdminProfilsUtilisateursLDAP { get; set; } = default!;

    // Entités Communes et liées aux dossiers d'homologation (similaires au Front Office)
    public DbSet<Attestation> Attestations { get; set; } = default!;
    public DbSet<CategorieEquipement> CategoriesEquipements { get; set; } = default!;
    public DbSet<Client> Clients { get; set; } = default!;
    public DbSet<Commentaire> Commentaires { get; set; } = default!;
    public DbSet<Demande> Demandes { get; set; } = default!;
    public DbSet<Devis> Devis { get; set; } = default!;
    public DbSet<DocumentDemande> DocumentsDemandes { get; set; } = default!;
    public DbSet<DocumentDossier> DocumentsDossiers { get; set; } = default!;
    public DbSet<Dossier> Dossiers { get; set; } = default!;
    public DbSet<ModeReglement> ModesReglements { get; set; } = default!;
    public DbSet<MotifRejet> MotifsRejets { get; set; } = default!;
    public DbSet<Proposition> Propositions { get; set; } = default!;
    public DbSet<Statut> Statuts { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; }

    /// <summary>
    /// Configure le modèle d'entités avec les mappings définis par les classes IEntityTypeConfiguration.
    /// Cette méthode est appelée lors de la création du modèle de base de données.
    /// </summary>
    /// <param name="builder">Le constructeur de modèle utilisé pour configurer le contexte.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Applique automatiquement toutes les configurations d'entités (classes *Configuration.cs)
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder); 
    }

    /// <summary>
    /// Configure les options du contexte de base de données.
    /// Utilisé ici pour ignorer un avertissement spécifique lors de l'utilisation de données de seed dynamiques.
    /// </summary>
    /// <param name="optionsBuilder">Constructeur d'options de DbContext.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        base.OnConfiguring(optionsBuilder); 
    }

    /// <summary>
    /// Override de la méthode de sauvegarde asynchrone pour implémenter la logique d'audit.
    /// Cette méthode est appelée avant chaque sauvegarde de changements dans la base de données.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le nombre d'objets écrits dans la base de données.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Tente de récupérer l'identifiant de l'utilisateur connecté via ICurrentUserService.
        var currentUserId = _currentUserService?.UserId?.ToString();

        // Parcourt toutes les entités qui héritent de AuditableEntity et qui sont suivies par le ChangeTracker.
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Pour une nouvelle entité, remplit les champs de création.
                    entry.Entity.UtilisateurCreation = currentUserId ?? "SYSTEM_BO";
                    entry.Entity.DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    break;

                case EntityState.Modified:
                    // Pour une entité modifiée, remplit les champs de modification.
                    entry.Entity.UtilisateurModification = currentUserId ?? "SYSTEM_BO";
                    entry.Entity.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    break;
            }
        }

        // Sauvegarde les modifications dans la base de données.
        return await base.SaveChangesAsync(cancellationToken);
    }
}