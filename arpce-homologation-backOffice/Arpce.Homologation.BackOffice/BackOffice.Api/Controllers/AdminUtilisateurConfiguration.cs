// Fichier : BackOffice.Infrastructure/Persistence/Configurations/AdminUtilisateurConfiguration.cs

using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Security.Cryptography; // Pour générer un mot de passe hashé temporaire

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class AdminUtilisateurConfiguration : IEntityTypeConfiguration<AdminUtilisateur>
{
    public void Configure(EntityTypeBuilder<AdminUtilisateur> builder)
    {
        builder.ToTable("AdminUtilisateurs");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Compte)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Compte).IsUnique();

        builder.Property(u => u.Nom)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Prenoms).HasMaxLength(150);
        builder.Property(u => u.MotPasse).HasMaxLength(255); 

        // Génére un hash de mot de passe pour l'administrateur
        var passwordHasher = new BackOffice.Infrastructure.Security.PasswordHasher(); 
        var adminPasswordHash = passwordHasher.Hash("AdminSecurePass123!"); 

        builder.HasData(
            new AdminUtilisateur
            {
                Id = Guid.NewGuid(), 
                Compte = "admin",
                Nom = "Administrateur",
                Prenoms = "Système",
                MotPasse = adminPasswordHash,
                Desactive = false,
                ChangementMotPasse = true, 
                DerniereConnexion = null
            }
        );
    }
}