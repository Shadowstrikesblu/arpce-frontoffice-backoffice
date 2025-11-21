using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Client.
/// Mappe la classe .NET à la table 'clients'.
/// </summary>
public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients"); // Nom de la table SQL

        builder.HasKey(c => c.Id); // Définition de la clé primaire

        builder.Property(c => c.Code)
            .HasMaxLength(30) // Longueur plus raisonnable pour un code client
            .IsRequired();

        builder.Property(c => c.RaisonSociale)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.RegistreCommerce).HasMaxLength(60);
        builder.Property(c => c.MotPasse).HasMaxLength(255); // Longueur pour hash de mot de passe
        builder.Property(c => c.ChangementMotPasse).HasColumnType("tinyint"); // Type SQL TINYINT
        builder.Property(c => c.Desactive).HasColumnType("tinyint"); // Type SQL TINYINT
        builder.Property(c => c.ContactNom).HasMaxLength(60);
        builder.Property(c => c.ContactTelephone).HasMaxLength(30);
        builder.Property(c => c.ContactFonction).HasMaxLength(60);
        builder.Property(c => c.Email).HasMaxLength(60);
        builder.Property(c => c.Adresse).HasMaxLength(512);
        builder.Property(c => c.Bp).HasMaxLength(60);
        builder.Property(c => c.Ville).HasMaxLength(60);
        builder.Property(c => c.Pays).HasMaxLength(60);
        builder.Property(c => c.Remarques).HasMaxLength(512);

        // Champs d'audit hérités de AuditableEntity
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("datetime");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("datetime");
    }
}