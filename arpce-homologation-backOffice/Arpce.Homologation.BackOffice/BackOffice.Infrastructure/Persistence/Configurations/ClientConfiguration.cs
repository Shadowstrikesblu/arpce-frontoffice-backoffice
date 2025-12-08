using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        // Clé primaire
        builder.HasKey(c => c.Id);

        // --- Propriétés d'identification ---
        builder.Property(c => c.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.RaisonSociale)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.RegistreCommerce)
            .HasMaxLength(60); // Optionnel

        // --- Propriétés de sécurité ---
        builder.Property(c => c.MotPasse)
            .HasMaxLength(255); // Hash du mot de passe

        builder.Property(c => c.ChangementMotPasse)
            .HasColumnType("tinyint"); // 0 ou 1

        builder.Property(c => c.Desactive)
            .HasColumnType("tinyint"); // 0 ou 1

        // --- Propriétés de contact ---
        builder.Property(c => c.ContactNom)
            .HasMaxLength(60);

        builder.Property(c => c.ContactTelephone)
            .HasMaxLength(30);

        builder.Property(c => c.ContactFonction)
            .HasMaxLength(60);

        builder.Property(c => c.Email)
            .HasMaxLength(120);

        // --- NOUVEAUX CHAMPS (Demande Front Office) ---
        builder.Property(c => c.Adresse)
            .HasMaxLength(255);

        builder.Property(c => c.Bp)
            .HasMaxLength(60);

        builder.Property(c => c.Ville)
            .HasMaxLength(100);

        builder.Property(c => c.Pays)
            .HasMaxLength(100);

        builder.Property(c => c.Remarques)
            .HasMaxLength(512);

        // Type de client ("Particulier" ou "Entreprise")
        builder.Property(c => c.TypeClient)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Entreprise");

        // --- CHAMPS DU WORKFLOW DE VALIDATION ---

        // Niveau de validation du compte :
        // 0 = Inscrit (en attente OTP)
        // 1 = OTP Validé (en attente Validation ARPCE)
        // 2 = Validé ARPCE (Compte Actif)
        builder.Property(c => c.NiveauValidation)
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);

        // IsVerified est conservé pour la compatibilité avec la logique OTP existante (passage de 0 à 1)
        builder.Property(c => c.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.VerificationCode)
            .HasMaxLength(6); // Code OTP à 6 chiffres

        builder.Property(c => c.VerificationTokenExpiry); // Date d'expiration de l'OTP

        // --- Champs d'audit ---
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");

        builder.Property(c => c.VerificationTokenExpiry).HasColumnType("bigint");
    }
}