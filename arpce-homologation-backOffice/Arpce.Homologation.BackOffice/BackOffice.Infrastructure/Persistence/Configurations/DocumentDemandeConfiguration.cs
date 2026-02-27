using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité DocumentDemande.
/// Mappe la classe .NET à la table 'documentsDemandes'.
/// </summary>
public class DocumentDemandeConfiguration : IEntityTypeConfiguration<DocumentDemande>
{
    public void Configure(EntityTypeBuilder<DocumentDemande> builder)
    {
        builder.ToTable("documentsDemandes"); 

        builder.HasKey(d => d.Id); 

        builder.Property(d => d.Nom).HasMaxLength(120); 
        builder.Property(d => d.Extension).HasMaxLength(12).IsRequired();
        builder.Property(d => d.Type).HasColumnType("tinyint");
        builder.Property(d => d.FilePath).HasMaxLength(512); 

        // Définition de la relation avec Demande
        builder.HasOne(d => d.Demande)
            .WithMany(demande => demande.DocumentsDemandes)
            .HasForeignKey(d => d.IdDemande)
            .OnDelete(DeleteBehavior.Cascade); 

        // Champs d'audit hérités de AuditableEntity
        builder.Property(c => c.UtilisateurCreation).HasMaxLength(60);
        builder.Property(c => c.DateCreation).HasColumnType("bigint");
        builder.Property(c => c.UtilisateurModification).HasMaxLength(60);
        builder.Property(c => c.DateModification).HasColumnType("bigint");
    }
}