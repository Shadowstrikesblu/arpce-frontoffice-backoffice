using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class AdminProfilsUtilisateursLDAPConfiguration : IEntityTypeConfiguration<AdminProfilsUtilisateursLDAP>
{
    public void Configure(EntityTypeBuilder<AdminProfilsUtilisateursLDAP> builder)
    {
        builder.ToTable("adminProfilsUtilisateursLDAP");
        builder.HasKey(apu => new { apu.Utilisateur, apu.IdProfil });

        builder.Property(apu => apu.Utilisateur).HasMaxLength(120).IsRequired();

        builder.HasOne(apu => apu.Profil).WithMany().HasForeignKey(apu => apu.IdProfil).OnDelete(DeleteBehavior.Cascade);
    }
}