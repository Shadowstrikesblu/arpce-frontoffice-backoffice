using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminEvenementsTypesConfiguration : IEntityTypeConfiguration<AdminEvenementsTypes>
{
    public void Configure(EntityTypeBuilder<AdminEvenementsTypes> builder)
    {
        builder.ToTable("adminEvenementsTypes");
        builder.Property(aet => aet.Id).IsRequired();

        builder.Property(aet => aet.Libelle).HasMaxLength(120).IsRequired().HasColumnType("nvarchar(120)"); 
    }
}