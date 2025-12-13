using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        builder.Property(d => d.PaiementMobileId).HasMaxLength(60);

        builder.HasOne(d => d.Demande)
     .WithMany(dem => dem.Devis) 
     .HasForeignKey(d => d.IdDemande);

    }
}