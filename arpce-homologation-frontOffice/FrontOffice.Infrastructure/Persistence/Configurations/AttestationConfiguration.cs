
using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FrontOffice.Infrastructure.Persistence.Configurations
{
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

            builder.Property(a => a.VisaReference)
                .HasMaxLength(100);

            builder.HasIndex(a => a.NumeroSequentiel);

            // Relation avec la Demande
            builder.HasOne(a => a.Demande)
                .WithMany(d => d.Attestations)
                .HasForeignKey(a => a.IdDemande)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Signataire)
                .WithMany()
                .HasForeignKey(a => a.SignataireId)
                .OnDelete(DeleteBehavior.Restrict);

            // Audit
            builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
            builder.Property(c => c.DateCreation).HasColumnType("bigint");
            builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
            builder.Property(c => c.DateModification).HasColumnType("bigint");
        }

    }
}
