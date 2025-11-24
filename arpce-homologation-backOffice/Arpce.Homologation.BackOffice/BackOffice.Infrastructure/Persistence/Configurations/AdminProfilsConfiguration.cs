using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

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
        builder.Property(ap => ap.DateCreation).HasColumnType("datetime");
        builder.Property(ap => ap.UtilisateurModification).HasMaxLength(60);
        builder.Property(ap => ap.DateModification).HasColumnType("datetime");
    }
}