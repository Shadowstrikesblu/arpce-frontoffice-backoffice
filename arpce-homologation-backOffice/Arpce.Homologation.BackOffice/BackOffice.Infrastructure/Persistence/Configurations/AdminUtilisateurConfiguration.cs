using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AdminUtilisateurConfiguration : IEntityTypeConfiguration<AdminUtilisateur>
{
    public void Configure(EntityTypeBuilder<AdminUtilisateur> builder)
    {
        builder.ToTable("AdminUtilisateurs");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Compte).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Compte).IsUnique();
        builder.Property(u => u.Nom).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Prenoms).HasMaxLength(150);
        builder.Property(u => u.MotPasse).HasMaxLength(255);

    }
}