using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité MotifRejet.
/// Mappe la classe .NET à la table 'motifsRejets'.
/// </summary>
public class MotifRejetConfiguration : IEntityTypeConfiguration<MotifRejet>
{
    public void Configure(EntityTypeBuilder<MotifRejet> builder)
    {
        builder.ToTable("motifsRejets");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Code)
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(m => m.Libelle)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(m => m.Remarques).HasMaxLength(512);

        // Champs d'audit hérités de AuditableEntity
        builder.Property(m => m.UtilisateurCreation).HasMaxLength(60);
        builder.Property(m => m.DateCreation).HasColumnType("bigint");
        builder.Property(m => m.UtilisateurModification).HasMaxLength(60);
        builder.Property(m => m.DateModification).HasColumnType("bigint");
    }
}