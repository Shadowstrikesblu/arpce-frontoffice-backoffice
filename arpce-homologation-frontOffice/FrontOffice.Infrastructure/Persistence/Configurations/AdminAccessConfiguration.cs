using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminAccessConfiguration : IEntityTypeConfiguration<AdminAccess>
{
    public void Configure(EntityTypeBuilder<AdminAccess> builder)
    {
        builder.ToTable("adminAccess");
        builder.HasKey(a => a.Id);

        
        builder.Property(a => a.Id).IsRequired();

        builder.Property(a => a.Application).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)");
        builder.Property(a => a.Groupe).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)");
        builder.Property(a => a.Libelle).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)");
        builder.Property(a => a.Page).HasMaxLength(120).HasColumnType("nvarchar(120)");
        builder.Property(a => a.Type).HasMaxLength(12).IsRequired().HasColumnType("nvarchar(12)");
        builder.Property(a => a.Inactif).HasColumnType("tinyint");
        builder.Property(a => a.Ajouter).HasColumnType("tinyint");
        builder.Property(a => a.Valider).HasColumnType("tinyint");
        builder.Property(a => a.Supprimer).HasColumnType("tinyint");
        builder.Property(a => a.Imprimer).HasColumnType("tinyint");
    }
}