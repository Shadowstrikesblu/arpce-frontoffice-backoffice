using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DemandeConfiguration : IEntityTypeConfiguration<Demande>
{
    public void Configure(EntityTypeBuilder<Demande> builder)
    {
        builder.ToTable("demandes");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.NumeroDemande).HasMaxLength(12);
        builder.Property(d => d.Equipement).HasMaxLength(120);
        builder.Property(d => d.Modele).HasMaxLength(120);
        builder.Property(d => d.Marque).HasMaxLength(120);
        builder.Property(d => d.Fabricant).HasMaxLength(120);
        builder.Property(d => d.Type).HasMaxLength(120);
        builder.Property(d => d.Description).HasMaxLength(512);
        builder.Property(d => d.ContactNom).HasMaxLength(60);
        builder.Property(d => d.ContactEmail).HasMaxLength(60);
        builder.HasOne(d => d.Dossier).WithMany(dossier => dossier.Demandes).HasForeignKey(d => d.IdDossier);
        builder.HasOne(d => d.CategorieEquipement).WithMany().HasForeignKey(d => d.IdCategorie).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.MotifRejet).WithMany().HasForeignKey(d => d.IdMotifRejet).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.Proposition).WithMany().HasForeignKey(d => d.IdProposition).OnDelete(DeleteBehavior.Restrict);
    }
}