using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Demande (représente un équipement à homologuer).
/// Mappe la classe .NET à la table 'demandes'.
/// </summary>
public class DemandeConfiguration : IEntityTypeConfiguration<Demande>
{
    public void Configure(EntityTypeBuilder<Demande> builder)
    {
        builder.ToTable("demandes"); 

        builder.HasKey(d => d.Id); 

        builder.Property(d => d.NumeroDemande).HasMaxLength(12);
        builder.Property(d => d.Equipement).HasMaxLength(120);
        builder.Property(d => d.Modele).HasMaxLength(120);
        builder.Property(d => d.Marque).HasMaxLength(120);
        builder.Property(d => d.Fabricant).HasMaxLength(120);
        builder.Property(d => d.Type).HasMaxLength(120);
        builder.Property(d => d.Description).HasMaxLength(512);
        builder.Property(d => d.ContactNom).HasMaxLength(60);
        builder.Property(d => d.ContactEmail).HasMaxLength(60);
        builder.Property(d => d.QuantiteEquipements).HasColumnType("int");

        builder.Property(d => d.PrixUnitaire).HasColumnType("money");

        builder.Property(d => d.Remise).HasColumnType("decimal(5, 2)");

        // Définition des relations
        builder.HasOne(d => d.Dossier)
            .WithMany(dossier => dossier.Demandes)
            .HasForeignKey(d => d.IdDossier)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(d => d.CategorieEquipement)
            .WithMany() 
            .HasForeignKey(d => d.IdCategorie)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(d => d.MotifRejet)
            .WithMany()
            .HasForeignKey(d => d.IdMotifRejet)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Proposition)
            .WithMany()
            .HasForeignKey(d => d.IdProposition)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");

        builder.Property(d => d.EstHomologable)
            .HasColumnType("bit") 
            .IsRequired()
            .HasDefaultValue(true);
    }
}