using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminUtilisateurTypesConfiguration : IEntityTypeConfiguration<AdminUtilisateurTypes>
{
    public void Configure(EntityTypeBuilder<AdminUtilisateurTypes> builder)
    {
        builder.ToTable("adminUtilisateurTypes");
        builder.HasKey(aut => aut.Id);

        builder.Property(aut => aut.Libelle).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)"); 
    }
}