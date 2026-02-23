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
        builder.Property(p => p.DateCreation)
               .HasColumnType("bigint");
        builder.Property(ap => ap.UtilisateurModification).HasMaxLength(60);
        builder.Property(p => p.DateModification)
                .HasColumnType("bigint");

        builder.HasData(
        new AdminProfils { Id = new Guid("11111111-1111-1111-1111-111111111111"), Code = "ADMIN", Libelle = "Administrateur" }
        //new AdminProfils { Id = new Guid("22222222-2222-2222-2222-222222222222"), Code = "DRSCE", Libelle = "Direction Technique" },
        //new AdminProfils { Id = new Guid("33333333-3333-3333-3333-333333333333"), Code = "DAFC", Libelle = "Direction Financière" }
    );
    }
}