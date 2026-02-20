using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Infrastructure.Persistence.Configurations
{
    public class BeneficiaireConfiguration : IEntityTypeConfiguration<Beneficiaire>
    {
        public void Configure(EntityTypeBuilder<Beneficiaire> builder)
        {
            builder.ToTable("beneficiaires");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nom).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(150);
            builder.Property(x => x.Telephone).HasMaxLength(50);

            // Relation 1:1 avec la Demande (Equipement)
            builder.HasOne(x => x.Demande)
                   .WithOne(x => x.Beneficiaire)
                   .HasForeignKey<Beneficiaire>(x => x.DemandeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
