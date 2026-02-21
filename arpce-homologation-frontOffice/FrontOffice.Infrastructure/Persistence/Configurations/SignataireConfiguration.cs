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
    public class SignataireConfiguration : IEntityTypeConfiguration<Signataire>
    {
        public void Configure(EntityTypeBuilder<Signataire> builder)
        {
            builder.ToTable("signataires");

            // L'Id de Signataire est la PK
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SignatureImagePath).HasMaxLength(512).IsRequired(false);
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            // CONFIGURATION DE L'EXTENSION (Relation 1:1)
            // L'Id de Signataire est aussi la Clé Étrangère vers AdminUtilisateur
            builder.HasOne(x => x.AdminUtilisateur)
                   .WithOne(u => u.Signataire)
                   .HasForeignKey<Signataire>(x => x.Id)
                   .OnDelete(DeleteBehavior.Restrict);

            // Audit
            builder.Property(x => x.UtilisateurCreation).HasMaxLength(60);
            builder.Property(x => x.DateCreation).HasColumnType("bigint");
            builder.Property(x => x.UtilisateurModification).HasMaxLength(60);
            builder.Property(x => x.DateModification).HasColumnType("bigint");
        }
    }
}
