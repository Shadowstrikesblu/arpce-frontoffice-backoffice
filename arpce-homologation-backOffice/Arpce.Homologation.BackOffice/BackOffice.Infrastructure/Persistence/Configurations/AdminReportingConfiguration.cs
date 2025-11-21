using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminReporting.
/// Mappe la classe .NET à la table 'adminReporting'.
/// </summary>
public class AdminReportingConfiguration : IEntityTypeConfiguration<AdminReporting>
{
    public void Configure(EntityTypeBuilder<AdminReporting> builder)
    {
        builder.ToTable("adminReporting");

        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Application).HasMaxLength(120);
        builder.Property(ar => ar.Code).HasMaxLength(12).IsRequired();
        builder.Property(ar => ar.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(ar => ar.Inactif).HasColumnType("tinyint");     
    }
}