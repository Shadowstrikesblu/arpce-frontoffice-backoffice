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

        var adminTypeId = Guid.Parse("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"); 

        builder.HasData(
            new AdminUtilisateurTypes { Id = adminTypeId, Libelle = "Administrateur" },
            new AdminUtilisateurTypes { Id = Guid.NewGuid(), Libelle = "Utilisateur Standard" }, 
            new AdminUtilisateurTypes { Id = Guid.NewGuid(), Libelle = "Auditeur" }
        );
    }
}