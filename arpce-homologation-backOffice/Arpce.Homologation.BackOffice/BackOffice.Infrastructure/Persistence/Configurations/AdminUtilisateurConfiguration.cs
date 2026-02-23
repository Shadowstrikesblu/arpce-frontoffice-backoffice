using BackOffice.Domain.Entities;
using BackOffice.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class AdminUtilisateurConfiguration : IEntityTypeConfiguration<AdminUtilisateur>
{
    public void Configure(EntityTypeBuilder<AdminUtilisateur> builder)
    {
        builder.ToTable("AdminUtilisateurs");

        builder.HasKey(u => u.Id);

        // Configuration du Compte (Login)
        builder.Property(u => u.Compte).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Compte).IsUnique(); // Le compte est unique

        // Configuration du Nom
        builder.Property(u => u.Nom).HasMaxLength(100).IsRequired();

        // --- AJOUT DEMANDÉ : Rendre le NOM unique ---
        builder.HasIndex(u => u.Nom).IsUnique();
        // -------------------------------------------

        builder.Property(u => u.Prenoms).HasMaxLength(150);
        builder.Property(u => u.MotPasse).HasMaxLength(255);

        builder.Property(u => u.Desactive).HasColumnType("bit");
        builder.Property(u => u.ChangementMotPasse).HasColumnType("bit");
        builder.Property(u => u.DerniereConnexion).HasColumnType("bigint");

        builder.Property(u => u.UtilisateurCreation).HasMaxLength(60);
        builder.Property(u => u.DateCreation).HasColumnType("bigint");
        builder.Property(u => u.UtilisateurModification).HasMaxLength(60);
        builder.Property(u => u.DateModification).HasColumnType("bigint");
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
        builder.Property(u => u.Email).HasMaxLength(255);

        var passwordHasher = new PasswordHasher();
        var adminPasswordHash = passwordHasher.Hash("admin.arpce@2025");

        // Utilisation de 'new Guid' avec une chaîne propre
        var adminTypeId = new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6");

        var adminUserId = new Guid("88888888-8888-8888-8888-888888888888");

        builder.HasData(
            new AdminUtilisateur
            {
                Id = adminUserId,
                IdUtilisateurType = adminTypeId,
                IdProfil = new Guid("11111111-1111-1111-1111-111111111111"),
                Compte = "admin",
                Nom = "root", 
                Prenoms = "ARPCE",
                MotPasse = adminPasswordHash,
                Desactive = false,
                ChangementMotPasse = true,
                DerniereConnexion = null,
                DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                UtilisateurCreation = "SYSTEM_SEED"
            }
        );
    }
}