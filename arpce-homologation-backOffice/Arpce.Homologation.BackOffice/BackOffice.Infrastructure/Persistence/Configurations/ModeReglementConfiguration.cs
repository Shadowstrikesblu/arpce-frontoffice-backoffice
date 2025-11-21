using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité ModeReglement.
/// Mappe la classe .NET à la table 'modesReglements'.
/// Inclut le seed de données initiales pour les modes de règlement.
/// </summary>
public class ModeReglementConfiguration : IEntityTypeConfiguration<ModeReglement>
{
    public void Configure(EntityTypeBuilder<ModeReglement> builder)
    {
        builder.ToTable("modesReglements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Code)
            .HasMaxLength(120) 
            .IsRequired();

        builder.Property(m => m.Libelle)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(m => m.MobileBanking)
            .HasColumnType("tinyint")
            .IsRequired();

        builder.Property(m => m.Remarques).HasMaxLength(512);

        // Champs d'audit hérités de AuditableEntity
        builder.Property(m => m.UtilisateurCreation).HasMaxLength(60);
        builder.Property(m => m.DateCreation).HasColumnType("datetime");
        builder.Property(m => m.UtilisateurModification).HasMaxLength(60);
        builder.Property(m => m.DateModification).HasColumnType("datetime");

        // --- Ajout des données initiales (Seed) ---
        builder.HasData(
     new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.Virement.ToString(), Libelle = "Virement bancaire", MobileBanking = 0 },
     new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.Cheque.ToString(), Libelle = "Chèque", MobileBanking = 0 },
     new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.Especes.ToString(), Libelle = "Espèces", MobileBanking = 0 },
     new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.MobileBanking.ToString(), Libelle = "Paiement mobile", MobileBanking = 1 }
 );
    }
}