using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminReportingConfiguration : IEntityTypeConfiguration<AdminReporting>
{
    public void Configure(EntityTypeBuilder<AdminReporting> builder)
    {
        builder.ToTable("adminReporting");
        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Application).HasMaxLength(120).HasColumnType("nvarchar(120)"); 
        builder.Property(ar => ar.Code).HasMaxLength(12).IsRequired().HasColumnType("nvarchar(12)"); 
        builder.Property(ar => ar.Libelle).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)"); 
        builder.Property(ar => ar.Inactif).HasColumnType("tinyint"); 
    }
}