using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminJournal.
/// Mappe la classe .NET à la table 'adminJournal'.
/// </summary>
public class AdminJournalConfiguration : IEntityTypeConfiguration<AdminJournal>
{
    public void Configure(EntityTypeBuilder<AdminJournal> builder)
    {
        builder.ToTable("adminJournal");

        builder.HasKey(aj => aj.Id);

        builder.Property(aj => aj.IdEvenementType).IsRequired();
        builder.Property(aj => aj.Application).HasMaxLength(120).IsRequired(); 
        builder.Property(aj => aj.AdresseIP).HasMaxLength(120).IsRequired();   
        builder.Property(aj => aj.Utilisateur).HasMaxLength(120).IsRequired(); 
        builder.Property(aj => aj.DateEvenement).HasColumnType("bigint").IsRequired(); 
        builder.Property(aj => aj.Page).HasMaxLength(120).IsRequired();        
        builder.Property(aj => aj.Libelle).HasMaxLength(255);                

        // Définition de la relation avec AdminEvenementsTypes
        builder.HasOne(aj => aj.EvenementType)
               .WithMany()
               .HasForeignKey(aj => aj.IdEvenementType)
               .OnDelete(DeleteBehavior.Restrict);
    }
}