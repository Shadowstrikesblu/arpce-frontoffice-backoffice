using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DossierConfiguration : IEntityTypeConfiguration<Dossier>
{
    public void Configure(EntityTypeBuilder<Dossier> builder)
    {
        builder.ToTable("dossiers");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Numero).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(d => d.DateOuverture).HasColumnType("bigint").IsRequired();

        builder.HasOne(d => d.Client)
            .WithMany(c => c.Dossiers)
            .HasForeignKey(d => d.IdClient)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Statut)
            .WithMany()
            .HasForeignKey(d => d.IdStatut)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.ModeReglement)
            .WithMany()
            .HasForeignKey(d => d.IdModeReglement)
            .IsRequired(false) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}