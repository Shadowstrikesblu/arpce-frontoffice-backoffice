using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PropositionConfiguration : IEntityTypeConfiguration<Proposition>
{
    public void Configure(EntityTypeBuilder<Proposition> builder)
    {
        builder.ToTable("propositions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(p => p.Libelle)
            .HasMaxLength(120)
            .IsRequired();

        // Champs d'audit hérités de AuditableEntity
        builder.Property(p => p.UtilisateurCreation).HasMaxLength(60);
        builder.Property(p => p.DateCreation).HasColumnType("bigint");
        builder.Property(p => p.UtilisateurModification).HasMaxLength(60);
        builder.Property(p => p.DateModification).HasColumnType("bigint");
    }
}