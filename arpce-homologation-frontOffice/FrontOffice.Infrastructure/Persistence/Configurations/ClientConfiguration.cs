using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Client.
/// Mappe la classe .NET à la table 'clients' dans la base de données
/// et configure les propriétés, longueurs et contraintes de chaque colonne.
/// </summary>
public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Définit le nom de la table dans la base de données.
        builder.ToTable("clients");

        // Définit la clé primaire de la table.
        builder.HasKey(c => c.Id);

        // --- Configuration des propriétés de l'entité Client ---

        builder.Property(c => c.Code)
            .HasMaxLength(30) 
            .IsRequired();    

        builder.Property(c => c.RaisonSociale)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.RegistreCommerce)
            .HasMaxLength(60); 

        builder.Property(c => c.MotPasse)
            .HasMaxLength(255); 

        builder.Property(c => c.ChangementMotPasse)
            .HasColumnType("tinyint"); 

        builder.Property(c => c.Desactive)
            .HasColumnType("tinyint");

        builder.Property(c => c.ContactNom)
            .HasMaxLength(60);

        builder.Property(c => c.ContactTelephone)
            .HasMaxLength(30);

        builder.Property(c => c.ContactFonction)
            .HasMaxLength(60);

        builder.Property(c => c.Email)
            .HasMaxLength(120); 

        builder.Property(c => c.Adresse)
            .HasMaxLength(512);

        builder.Property(c => c.Bp)
            .HasMaxLength(60);

        builder.Property(c => c.Ville)
            .HasMaxLength(60);

        builder.Property(c => c.Pays)
            .HasMaxLength(60);

        builder.Property(c => c.Remarques)
            .HasMaxLength(512);

        // La valeur par défaut pour un nouveau client sera 'false' (0) au niveau de la base de données.
        builder.Property(c => c.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);

        // Le code de vérification à 6 chiffres, stocké comme une chaîne de caractères.
        builder.Property(c => c.VerificationCode)
            .HasMaxLength(6);

        // La date d'expiration sera est un 'datetime' 
        builder.Property(c => c.VerificationTokenExpiry);


        // --- Configuration des champs d'audit hérités de AuditableEntity ---

        builder.Property(c => c.UtilisateurCreation)
            .HasMaxLength(60);

        builder.Property(c => c.DateCreation)
            .HasColumnType("datetime"); 

        builder.Property(c => c.UtilisateurModification)
            .HasMaxLength(60);

        builder.Property(c => c.DateModification)
            .HasColumnType("datetime");
    }
}