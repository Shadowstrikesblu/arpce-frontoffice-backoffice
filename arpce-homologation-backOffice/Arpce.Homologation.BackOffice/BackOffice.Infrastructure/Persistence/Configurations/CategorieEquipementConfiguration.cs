using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité CategorieEquipement.
/// Mappe la classe .NET à la table 'categoriesEquipements'.
/// </summary>
public class CategorieEquipementConfiguration : IEntityTypeConfiguration<CategorieEquipement>
{
    public void Configure(EntityTypeBuilder<CategorieEquipement> builder)
    {
        builder.ToTable("categoriesEquipements"); 

        builder.HasKey(c => c.Id); 

        builder.Property(c => c.Code)
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(c => c.Libelle)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(c => c.TarifEtude).HasColumnType("money"); 
        builder.Property(c => c.TarifHomologation).HasColumnType("money");
        builder.Property(c => c.TarifHomologationParLot).HasColumnType("tinyint"); 
        builder.Property(c => c.TarifHomologationQuantiteParLot).HasColumnType("int"); 
        builder.Property(c => c.TarifControle).HasColumnType("money");
        builder.Property(c => c.Remarques).HasMaxLength(512);

        // Champs d'audit hérités de AuditableEntity
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("datetime"); 
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("datetime");
    }
}