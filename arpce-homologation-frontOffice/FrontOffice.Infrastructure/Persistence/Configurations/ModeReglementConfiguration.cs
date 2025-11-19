using FrontOffice.Domain.Entities;
using FrontOffice.Domain.Enums; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 

public class ModeReglementConfiguration : IEntityTypeConfiguration<ModeReglement>
{
    public void Configure(EntityTypeBuilder<ModeReglement> builder)
    {
        builder.ToTable("modesReglements");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Code).HasMaxLength(120).IsRequired(); 
        builder.Property(m => m.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(m => m.MobileBanking).IsRequired();
        builder.Property(m => m.Remarques).HasMaxLength(512);
        builder.Property(m => m.UtilisateurCreation).HasMaxLength(60);
        builder.Property(m => m.DateCreation).HasColumnType("smalldatetime");
        builder.Property(m => m.UtilisateurModification).HasMaxLength(60);
        builder.Property(m => m.DateModification).HasColumnType("smalldatetime");

        // --- Ajout des données initiales (Seed) ---
        builder.HasData(
            new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.Virement.ToString(), Libelle = "Virement bancaire", MobileBanking = 0 },
            new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.Cheque.ToString(), Libelle = "Chèque", MobileBanking = 0 },
            new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.Especes.ToString(), Libelle = "Espèces", MobileBanking = 0 },
            new ModeReglement { Id = Guid.NewGuid(), Code = ModeReglementEnum.MobileBanking.ToString(), Libelle = "Paiement mobile", MobileBanking = 1 }
        );
    }
}