using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Attestation.
/// Mappe la classe .NET à la table 'attestations'.
/// </summary>
public class AttestationConfiguration : IEntityTypeConfiguration<Attestation>
{
    public void Configure(EntityTypeBuilder<Attestation> builder)
    {
        builder.ToTable("attestations");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Extension)
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(a => a.DateDelivrance)
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(a => a.DateExpiration)
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(a => a.NumeroSequentiel)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(a => a.NumeroSequentiel);

        builder.HasOne(a => a.Demande)
            .WithMany(d => d.Attestations)
            .HasForeignKey(a => a.IdDemande)
            .OnDelete(DeleteBehavior.Cascade);
    }
}