using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class CategorieEquipementConfiguration : IEntityTypeConfiguration<CategorieEquipement>
{
    public void Configure(EntityTypeBuilder<CategorieEquipement> builder)
    {
        builder.ToTable("categoriesEquipements");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).HasMaxLength(12).IsRequired();
        builder.Property(c => c.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(c => c.TypeEquipement).HasMaxLength(50);
        builder.Property(c => c.TypeClient).HasMaxLength(50);
        builder.Property(c => c.FormuleHomologation).HasMaxLength(255);

        builder.Property(c => c.FraisEtude).HasColumnType("money");
        builder.Property(c => c.FraisHomologation).HasColumnType("money");
        builder.Property(c => c.FraisControle).HasColumnType("money");

        builder.Property(c => c.FraisHomologationParLot).HasColumnType("tinyint");
        builder.Property(c => c.FraisHomologationQuantiteParLot).HasColumnType("int");

        builder.Property(c => c.Remarques).HasMaxLength(512);

        // Champs d'audit
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }

}