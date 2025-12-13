using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminJournalConfiguration : IEntityTypeConfiguration<AdminJournal>
{
    public void Configure(EntityTypeBuilder<AdminJournal> builder)
    {
        builder.ToTable("adminJournal");
        builder.HasKey(aj => aj.Id);

        builder.Property(aj => aj.Id).IsRequired(); 
        builder.Property(aj => aj.IdEvenementType).IsRequired(); 
        builder.Property(aj => aj.Application).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)");
        builder.Property(aj => aj.AdresseIP).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)");
        builder.Property(aj => aj.DateEvenement).HasColumnType("bigint").IsRequired();
        builder.Property(aj => aj.Page).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)");
        builder.Property(aj => aj.Libelle).HasMaxLength(255).HasColumnType("nvarchar(255)");

        builder.HasOne(aj => aj.EvenementType)
               .WithMany()
               .HasForeignKey(aj => aj.IdEvenementType)
               .OnDelete(DeleteBehavior.Restrict);
    }
}