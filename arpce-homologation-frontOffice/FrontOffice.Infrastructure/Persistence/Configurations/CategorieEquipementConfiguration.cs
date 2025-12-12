using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategorieEquipementConfiguration : IEntityTypeConfiguration<CategorieEquipement>
{
    public void Configure(EntityTypeBuilder<CategorieEquipement> builder)
    {
        builder.ToTable("categoriesEquipements");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).HasMaxLength(12).IsRequired();
        builder.Property(c => c.Libelle).HasMaxLength(120).IsRequired();
        builder.Property(c => c.FraisEtude).HasColumnType("money");
        builder.Property(c => c.FraisHomologation).HasColumnType("money");
        builder.Property(c => c.FraisControle).HasColumnType("money");
        builder.Property(c => c.Remarques).HasMaxLength(512);
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }
}