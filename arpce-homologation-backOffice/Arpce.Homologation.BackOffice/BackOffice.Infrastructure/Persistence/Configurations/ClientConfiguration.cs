using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).HasMaxLength(30).IsRequired();
        builder.Property(c => c.RaisonSociale).HasMaxLength(120).IsRequired();
        builder.Property(c => c.RegistreCommerce).HasMaxLength(60);
        builder.Property(c => c.MotPasse).HasMaxLength(255);
        builder.Property(c => c.ChangementMotPasse).HasColumnType("tinyint");
        builder.Property(c => c.Desactive).HasColumnType("tinyint");
        builder.Property(c => c.ContactNom).HasMaxLength(60);
        builder.Property(c => c.ContactTelephone).HasMaxLength(30);
        builder.Property(c => c.ContactFonction).HasMaxLength(60);
        builder.Property(c => c.Email).HasMaxLength(120);
        builder.Property(c => c.Adresse).HasMaxLength(512);
        builder.Property(c => c.Bp).HasMaxLength(60);
        builder.Property(c => c.Ville).HasMaxLength(60);
        builder.Property(c => c.Pays).HasMaxLength(60);
        builder.Property(c => c.Remarques).HasMaxLength(512);

        builder.Property(c => c.IsVerified)
            .IsRequired()
            .HasDefaultValue(false); 

        builder.Property(c => c.VerificationCode).HasMaxLength(6);
        builder.Property(c => c.VerificationTokenExpiry);

        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("datetime");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("datetime");
    }
}