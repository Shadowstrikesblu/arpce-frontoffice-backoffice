using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackOffice.Infrastructure.Security; 

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminUtilisateur.
/// Mappe la classe .NET à la table 'AdminUtilisateurs' dans la base de données.
/// </summary>
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

        // Le mot de passe sera un hash, donc une longueur plus importante est nécessaire.
        builder.Property(u => u.MotPasse).HasMaxLength(255);

        builder.Property(u => u.Desactive).HasColumnType("bit"); 
        builder.Property(u => u.ChangementMotPasse).HasColumnType("bit"); 
        builder.Property(u => u.DerniereConnexion).HasColumnType("datetime"); 

        var passwordHasher = new PasswordHasher(); 
        var adminPasswordHash = passwordHasher.Hash("admin.arpce@2025");

        //var adminId = Guid.Parse("eedf5684-9568-40f2-9b1a-3ca0ef23e1b9");

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