using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class DevisConfiguration : IEntityTypeConfiguration<Devis>
{
    public void Configure(EntityTypeBuilder<Devis> builder)
    {
        builder.ToTable("devis");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Date).HasColumnType("bigint").IsRequired();
        builder.Property(d => d.MontantEtude).HasColumnType("money").IsRequired();
        builder.Property(d => d.MontantHomologation).HasColumnType("money");
        builder.Property(d => d.MontantControle).HasColumnType("money");
        builder.Property(d => d.PaiementOk).HasColumnType("tinyint");
        builder.Property(d => d.PaiementMobileId).HasMaxLength(60);

        // Relation principale avec Dossier (comme dans le Back Office)
        builder.HasOne(d => d.Dossier)
            .WithMany(dossier => dossier.Devis)
            .HasForeignKey(d => d.IdDossier)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation optionnelle avec Demande
        builder.HasOne(d => d.Demande)
            .WithMany(demande => demande.Devis)
            .HasForeignKey(d => d.IdDemande)
            .IsRequired(false) 
            .OnDelete(DeleteBehavior.NoAction); // Pour éviter les cycles de suppression

        // Champs d'audit (si AuditableEntity les inclut)
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }
}