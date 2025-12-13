using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminConnexionsConfiguration : IEntityTypeConfiguration<AdminConnexions>
{
    public void Configure(EntityTypeBuilder<AdminConnexions> builder)
    {
        builder.ToTable("adminConnexions");
        
        builder.HasKey(ac => new { ac.Utilisateur, ac.DateConnexion }); 

        builder.Property(ac => ac.Utilisateur).HasMaxLength(60).IsRequired().HasColumnType("nvarchar(60)"); 
        builder.Property(ac => ac.DateConnexion).HasColumnType("bigint").IsRequired(); 
        builder.Property(ac => ac.Ip).HasMaxLength(60).HasColumnType("nvarchar(60)"); 
    }
}