using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité DocumentDossier.
/// Mappe la classe .NET à la table 'documentsDossiers'.
/// </summary>
public class DocumentDossierConfiguration : IEntityTypeConfiguration<DocumentDossier>
{
    public void Configure(EntityTypeBuilder<DocumentDossier> builder)
    {
        builder.ToTable("documentsDossiers"); 

        builder.HasKey(d => d.Id); 

        builder.Property(d => d.Nom).HasMaxLength(120); 
        builder.Property(d => d.Extension).HasMaxLength(12).IsRequired();
        builder.Property(d => d.Type).HasColumnType("tinyint"); 
        builder.Property(d => d.FilePath).HasMaxLength(512); 

        // Définition de la relation avec Dossier
        builder.HasOne(d => d.Dossier)
            .WithMany(dossier => dossier.DocumentsDossiers)
            .HasForeignKey(d => d.IdDossier)
            .OnDelete(DeleteBehavior.Cascade); 

        // Champs d'audit hérités de AuditableEntity
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("datetime");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("datetime");
    }
}