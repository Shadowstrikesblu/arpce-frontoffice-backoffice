using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

public class CategorieEquipementConfiguration : IEntityTypeConfiguration<CategorieEquipement>
{
    public void Configure(EntityTypeBuilder<CategorieEquipement> builder)
    {
        builder.ToTable("categoriesEquipements");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).HasMaxLength(12).IsRequired();
        builder.Property(c => c.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(c => c.TypeEquipement).HasMaxLength(50);

        builder.Property(c => c.TypeClient).HasMaxLength(50).IsRequired();

        builder.Property(c => c.FormuleHomologation).HasMaxLength(255);

        builder.Property(c => c.FraisEtude).HasColumnType("money");
        builder.Property(c => c.FraisHomologation).HasColumnType("money");
        builder.Property(c => c.FraisControle).HasColumnType("money");

        builder.Property(c => c.FraisHomologationParLot).HasColumnType("tinyint");
        builder.Property(c => c.FraisHomologationQuantiteParLot).HasColumnType("int");
        builder.Property(c => c.QuantiteReference).HasColumnType("int");

        builder.Property(c => c.CoutUnitaire).HasPrecision(18, 2).HasDefaultValue(0);
        builder.Property(c => c.TypeCalcul).HasMaxLength(20).HasDefaultValue("FORFAIT");
        builder.Property(c => c.ReferenceLoiFinance).HasMaxLength(100);
        builder.Property(c => c.QtyMin).HasColumnType("int");
        builder.Property(c => c.QtyMax).HasColumnType("int");
        builder.Property(c => c.BlockSize).HasColumnType("int");

        builder.Property(c => c.ModeCalcul)
            .HasConversion<int>()
            .HasDefaultValue(ModeCalcul.FIXED);

        builder.Property(c => c.Remarques).HasMaxLength(512);

        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }
}