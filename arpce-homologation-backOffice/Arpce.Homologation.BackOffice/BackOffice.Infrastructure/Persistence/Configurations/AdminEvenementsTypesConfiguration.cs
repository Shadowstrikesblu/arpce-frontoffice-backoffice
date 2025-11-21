using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminEvenementsTypes.
/// Mappe la classe .NET à la table 'adminEvenementsTypes'.
/// </summary>
public class AdminEvenementsTypesConfiguration : IEntityTypeConfiguration<AdminEvenementsTypes>
{
    public void Configure(EntityTypeBuilder<AdminEvenementsTypes> builder)
    {
        builder.ToTable("adminEvenementsTypes");

        builder.HasKey(aet => aet.Id);

        builder.Property(aet => aet.Libelle).HasMaxLength(120).IsRequired(); 
    }
}