using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminConnexions.
/// Mappe la classe .NET à la table 'adminConnexions'.
/// </summary>
public class AdminConnexionsConfiguration : IEntityTypeConfiguration<AdminConnexions>
{
    public void Configure(EntityTypeBuilder<AdminConnexions> builder)
    {
        builder.ToTable("adminConnexions");

        builder.HasKey(ac => ac.Id); 

        builder.Property(ac => ac.Utilisateur).HasMaxLength(60).IsRequired(); 
        builder.Property(ac => ac.DateConnexion).HasColumnType("bigint").IsRequired(); 
        builder.Property(ac => ac.Ip).HasMaxLength(60);                                 
    }
}