using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Statut.
/// Mappe la classe .NET à la table 'statuts'.
/// Inclut le seed de données initiales pour les statuts de dossier.
/// </summary>
public class StatutConfiguration : IEntityTypeConfiguration<Statut>
{
    public void Configure(EntityTypeBuilder<Statut> builder)
    {
        builder.ToTable("statuts");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .HasMaxLength(120) 
            .IsRequired();

        builder.Property(s => s.Libelle)
            .HasMaxLength(120)
            .IsRequired();

        // --- Ajout des données initiales (Seed) ---
        builder.HasData(
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.NouvelleDemande.ToString(), Libelle = "Nouvelle demande" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.EnCoursInstruction.ToString(), Libelle = "En cours d'instruction" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.EnvoyePourApprobation.ToString(), Libelle = "Envoyé pour approbation" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.ApprouveAttentePaiement.ToString(), Libelle = "Approuvé, en attente de paiement" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.Rejetee.ToString(), Libelle = "Rejetée" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.EquipementNonSoumisAHomologation.ToString(), Libelle = "Équipement non soumis à homologation" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.ApprouvePaiementEffectue.ToString(), Libelle = "Approuvé, paiement effectué" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.ApprouveAttestationSignee.ToString(), Libelle = "Approuvé, attestation signée" },
            new Statut { Id = Guid.NewGuid(), Code = StatutDossierEnum.AnnulationInstruction.ToString(), Libelle = "Annulation de l'instruction" }
        );
    }
}