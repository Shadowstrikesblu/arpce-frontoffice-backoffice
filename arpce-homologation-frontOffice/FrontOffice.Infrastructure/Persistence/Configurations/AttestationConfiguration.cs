
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
            builder.Property(a => a.Extension).HasMaxLength(12).IsRequired();
            builder.Property(a => a.DateDelivrance).HasColumnType("date").IsRequired();
            builder.Property(a => a.DateExpiration).HasColumnType("date").IsRequired();
            builder.HasOne(a => a.Demande).WithMany(d => d.Attestations).HasForeignKey(a => a.IdDemande);
        }

    }
}
