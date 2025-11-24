using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class AdminProfilsAccesConfiguration : IEntityTypeConfiguration<AdminProfilsAcces>
{
    public void Configure(EntityTypeBuilder<AdminProfilsAcces> builder)
    {
        builder.ToTable("adminProfilsAcces");
        builder.HasKey(apa => new { apa.IdProfil, apa.IdAccess });

        builder.Property(apa => apa.Ajouter).HasColumnType("tinyint");
        builder.Property(apa => apa.Valider).HasColumnType("tinyint");
        builder.Property(apa => apa.Supprimer).HasColumnType("tinyint");
        builder.Property(apa => apa.Imprimer).HasColumnType("tinyint");

        builder.HasOne(apa => apa.Profil).WithMany().HasForeignKey(apa => apa.IdProfil).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(apa => apa.Access).WithMany().HasForeignKey(apa => apa.IdAccess).OnDelete(DeleteBehavior.Cascade);
    }
}