using BackOffice.Domain.Entities;
using BackOffice.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class AdminUtilisateurConfiguration : IEntityTypeConfiguration<AdminUtilisateur>
{
    public void Configure(EntityTypeBuilder<AdminUtilisateur> builder)
    {
        builder.ToTable("AdminUtilisateurs");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Compte).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Compte).IsUnique();

        builder.Property(u => u.Nom).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Prenoms).HasMaxLength(150);
        builder.Property(u => u.MotPasse).HasMaxLength(255);

        builder.Property(u => u.Desactive).HasColumnType("bit");
        builder.Property(u => u.ChangementMotPasse).HasColumnType("bit");
        builder.Property(u => u.DerniereConnexion).HasColumnType("datetime");

        builder.Property(u => u.UtilisateurCreation).HasMaxLength(60);
        builder.Property(u => u.DateCreation).HasColumnType("datetime");
        builder.Property(u => u.UtilisateurModification).HasMaxLength(60);
        builder.Property(u => u.DateModification).HasColumnType("datetime");
        builder.Property(u => u.Remarques).HasMaxLength(512);

        builder.Property(u => u.IdProfil).IsRequired(false); 
        builder.HasOne(u => u.Profil)
            .WithMany() 
            .HasForeignKey(u => u.IdProfil)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(u => u.IdUtilisateurType).IsRequired(); 

        builder.HasOne(u => u.UtilisateurType)
            .WithMany() 
            .HasForeignKey(u => u.IdUtilisateurType)
            .OnDelete(DeleteBehavior.Restrict);

        var passwordHasher = new PasswordHasher();
        var adminPasswordHash = passwordHasher.Hash("admin.arpce@2025");

        var adminTypeId = Guid.Parse("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6");

        builder.HasData(
            new AdminUtilisateur
            {
                Id = Guid.NewGuid(),
                IdUtilisateurType = adminTypeId, 
                IdProfil = null, 
                Compte = "admin",
                Nom = "Administrateur",
                Prenoms = "ARPCE",
                MotPasse = adminPasswordHash,
                Desactive = false,
                ChangementMotPasse = true,
                DerniereConnexion = null,
                DateCreation = DateTime.Parse("2025-01-01"),
                UtilisateurCreation = "SYSTEM_SEED"
            }
        );
    }
}