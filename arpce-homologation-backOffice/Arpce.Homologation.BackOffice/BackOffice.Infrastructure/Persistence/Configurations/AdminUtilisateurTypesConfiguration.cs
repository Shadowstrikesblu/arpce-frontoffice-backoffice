using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminUtilisateurTypes.
/// Mappe la classe .NET à la table 'adminUtilisateurTypes'.
/// </summary>
public class AdminUtilisateurTypesConfiguration : IEntityTypeConfiguration<AdminUtilisateurTypes>
{
    public void Configure(EntityTypeBuilder<AdminUtilisateurTypes> builder)
    {
        builder.ToTable("adminUtilisateurTypes");

        builder.HasKey(aut => aut.Id);

        builder.Property(aut => aut.Libelle).HasMaxLength(120).IsRequired(); 
    }
}