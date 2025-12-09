using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Devis.
/// Mappe la classe .NET à la table 'devis'.
/// </summary>
public class DevisConfiguration : IEntityTypeConfiguration<Devis>
{
    public void Configure(EntityTypeBuilder<Devis> builder)
    {
        builder.ToTable("devis"); 

        builder.HasKey(d => d.Id); 

        builder.Property(d => d.Date)
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(d => d.MontantEtude)
            .HasColumnType("money")
            .IsRequired();

        builder.Property(d => d.MontantHomologation).HasColumnType("money");
        builder.Property(d => d.MontantControle).HasColumnType("money");
        builder.Property(d => d.PaiementOk).HasColumnType("tinyint"); 
        builder.Property(d => d.PaiementMobileId).HasMaxLength(60);

        // Définition de la relation avec Dossier
        builder.HasOne(d => d.Dossier)
            .WithMany(dossier => dossier.Devis)
            .HasForeignKey(d => d.IdDossier)
            .OnDelete(DeleteBehavior.Cascade); 

        // Champs d'audit hérités de AuditableEntity
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }
}