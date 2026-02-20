using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class SignataireConfiguration : IEntityTypeConfiguration<Signataire>
{
    public void Configure(EntityTypeBuilder<Signataire> builder)
    {
        builder.ToTable("signataires");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nom).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Prenoms).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Fonction).HasMaxLength(150).IsRequired();
        builder.Property(x => x.SignatureImagePath).HasMaxLength(512);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        // Relation avec l'utilisateur Admin qui possède cette signature
        builder.HasOne<AdminUtilisateur>()
               .WithMany()
               .HasForeignKey("AdminId")
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // Champs d'audit
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }
}