using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class DossierConfiguration : IEntityTypeConfiguration<Dossier>
{
    public void Configure(EntityTypeBuilder<Dossier> builder)
    {
        builder.ToTable("dossiers");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Numero).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(d => d.DateOuverture).HasColumnType("bigint").IsRequired();

        builder.Property(d => d.DateEnvoiDevis).HasColumnType("bigint");
        builder.Property(d => d.DateDemandeEchantillon).HasColumnType("bigint");
        builder.Property(d => d.PenaliteAppliquee).HasColumnType("bit").HasDefaultValue(false);

        builder.Property(d => d.RappelPaiementEnvoye).HasColumnType("bit").HasDefaultValue(false);
        builder.Property(d => d.RappelEchantillonEnvoye).HasColumnType("bit").HasDefaultValue(false);

        builder.HasOne(d => d.Client).WithMany(c => c.Dossiers).HasForeignKey(d => d.IdClient).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.Statut).WithMany().HasForeignKey(d => d.IdStatut).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.ModeReglement).WithMany().HasForeignKey(d => d.IdModeReglement).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<AdminUtilisateur>().WithMany().HasForeignKey(d => d.IdAgentInstructeur).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.Demande).WithOne(dem => dem.Dossier).HasForeignKey<Demande>(dem => dem.IdDossier).OnDelete(DeleteBehavior.Cascade);

        // Audit
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }
}