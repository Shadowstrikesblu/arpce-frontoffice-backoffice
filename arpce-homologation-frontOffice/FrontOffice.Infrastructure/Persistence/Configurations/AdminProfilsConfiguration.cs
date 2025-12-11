using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminProfilsConfiguration : IEntityTypeConfiguration<AdminProfils>
{
    public void Configure(EntityTypeBuilder<AdminProfils> builder)
    {
        builder.ToTable("adminProfils");
        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Code).HasMaxLength(12);
        builder.Property(ap => ap.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(ap => ap.Remarques).HasMaxLength(512);

        // Audit
        builder.Property(ap => ap.UtilisateurCreation).HasMaxLength(60);
        builder.Property(p => p.DateCreation)
               .HasColumnType("bigint");
        builder.Property(ap => ap.UtilisateurModification).HasMaxLength(60);
        builder.Property(p => p.DateModification)
                .HasColumnType("bigint");
    }
}