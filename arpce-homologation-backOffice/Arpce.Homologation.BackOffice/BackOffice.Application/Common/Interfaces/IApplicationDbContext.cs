using BackOffice.Domain.Entities; 
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.Interfaces;

/// <summary>
/// Définit le contrat pour le contexte de base de données du Back Office.
/// Déclare tous les ensembles d'entités (DbSet) que le Back Office va manipuler.
/// </summary>
public interface IApplicationDbContext
{
    // --- Entités d'administration ---
    DbSet<AdminAccess> AdminAccesses { get; }
    DbSet<AdminConnexions> AdminConnexions { get; }
    DbSet<AdminEvenementsTypes> AdminEvenementsTypes { get; }
    DbSet<AdminJournal> AdminJournals { get; }
    DbSet<AdminOptions> AdminOptions { get; }
    DbSet<AdminProfils> AdminProfils { get; }
    DbSet<AdminProfilsAcces> AdminProfilsAcces { get; }
    DbSet<AdminProfilsUtilisateursLDAP> AdminProfilsUtilisateursLDAP { get; }
    DbSet<AdminReporting> AdminReportings { get; }
    DbSet<AdminUtilisateur> AdminUtilisateurs { get; }
    DbSet<AdminUtilisateurTypes> AdminUtilisateurTypes { get; }

    // --- Entités Communes (Dossiers et leurs dépendances) ---
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

    /// <summary>
    /// Sauvegarde toutes les modifications en attente dans la base de données.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le nombre d'objets écrits dans la base de données.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}