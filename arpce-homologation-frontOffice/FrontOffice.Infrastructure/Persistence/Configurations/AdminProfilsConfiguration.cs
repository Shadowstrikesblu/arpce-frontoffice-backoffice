using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminProfilsConfiguration : IEntityTypeConfiguration<AdminProfils>
{
    public void Configure(EntityTypeBuilder<AdminProfils> builder)
    {
        builder.ToTable("adminProfils");
        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Code).HasMaxLength(12).HasColumnType("nvarchar(12)"); 
        builder.Property(ap => ap.Libelle).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)"); 
        builder.Property(ap => ap.Remarques).HasMaxLength(512).HasColumnType("nvarchar(512)"); 
        builder.Property(ap => ap.UtilisateurCreation).HasMaxLength(60).HasColumnType("nvarchar(60)"); 
        builder.Property(ap => ap.DateCreation).HasColumnType("smalldatetime"); 
        builder.Property(ap => ap.UtilisateurModification).HasMaxLength(60).HasColumnType("nvarchar(60)"); 
        builder.Property(ap => ap.DateModification).HasColumnType("smalldatetime"); 
    }
}