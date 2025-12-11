using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminAccess.
/// Mappe la classe .NET à la table 'adminAccess'.
/// </summary>
public class AdminAccessConfiguration : IEntityTypeConfiguration<AdminAccess>
{
    public void Configure(EntityTypeBuilder<AdminAccess> builder)
    {
        builder.ToTable("adminAccess");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Application).HasMaxLength(120).IsRequired(); 
        builder.Property(a => a.Groupe).HasMaxLength(120).IsRequired();      
        builder.Property(a => a.Libelle).HasMaxLength(120).IsRequired();     
        builder.Property(a => a.Page).HasMaxLength(120);                   
        builder.Property(a => a.Type).HasMaxLength(120).IsRequired();        

        builder.Property(a => a.Inactif).HasColumnType("tinyint");      
        builder.Property(a => a.Ajouter).HasColumnType("tinyint");      
        builder.Property(a => a.Valider).HasColumnType("tinyint");      
        builder.Property(a => a.Supprimer).HasColumnType("tinyint");    
        builder.Property(a => a.Imprimer).HasColumnType("tinyint");     
    }
}