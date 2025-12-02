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

        builder.HasData(
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "CREATION", Libelle = "Création de données" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "MODIFICATION", Libelle = "Modification de données" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "SUPPRESSION", Libelle = "Suppression de données" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "VALIDATION", Libelle = "Validation de processus" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "CONNEXION", Libelle = "Modification" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "MODIFICATION", Libelle = "Connexion utilisateur" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "ATTRIBUTION", Libelle = "Attribution de droits/profils" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "SECURITE", Libelle = "Action de sécurité" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "COMMUNICATION", Libelle = "Envoi de communication" },
            new AdminEvenementsTypes { Id = Guid.NewGuid(), Code = "QUALIFICATION", Libelle = "Qualification de données" }
        );
    }
}