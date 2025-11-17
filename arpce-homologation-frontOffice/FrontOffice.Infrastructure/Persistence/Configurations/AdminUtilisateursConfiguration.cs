using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class AdminUtilisateursConfiguration : IEntityTypeConfiguration<AdminUtilisateurs>
{
    public void Configure(EntityTypeBuilder<AdminUtilisateurs> builder)
    {
        builder.ToTable("adminUtilisateurs");
        builder.HasKey(au => au.Id);

        builder.Property(au => au.Compte).HasMaxLength(60).IsRequired().HasColumnType("nvarchar(60)"); 
        builder.Property(au => au.Nom).HasMaxLength(60).IsRequired().HasColumnType("nvarchar(60)"); 
        builder.Property(au => au.Prenoms).HasMaxLength(60).HasColumnType("nvarchar(60)"); 
        builder.Property(au => au.MotPasse).HasMaxLength(20).HasColumnType("nvarchar(20)"); 
        builder.Property(au => au.ChangementMotPasse).HasColumnType("tinyint"); 
        builder.Property(au => au.Desactive).HasColumnType("tinyint"); 
        builder.Property(au => au.Remarques).HasMaxLength(512).HasColumnType("nvarchar(512)"); 
        builder.Property(au => au.DerniereConnexion).HasColumnType("smalldatetime"); 
        builder.Property(au => au.UtilisateurCreation).HasMaxLength(60).HasColumnType("nvarchar(60)");
        builder.Property(au => au.DateCreation).HasColumnType("smalldatetime"); 
        builder.Property(au => au.UtilisateurModification).HasMaxLength(60).HasColumnType("nvarchar(60)"); 
        builder.Property(au => au.DateModification).HasColumnType("smalldatetime"); 

        // Relations
        builder.HasOne(au => au.UtilisateurType)
               .WithMany() 
               .HasForeignKey(au => au.IdUtilisateurType)
               .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(au => au.Profil)
               .WithMany() 
               .HasForeignKey(au => au.IdProfil)
               .OnDelete(DeleteBehavior.Restrict); 
    }
}