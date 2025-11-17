// Fichier : FrontOffice.Infrastructure/Persistence/Configurations/DocumentDossierConfiguration.cs
using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DocumentDossierConfiguration : IEntityTypeConfiguration<DocumentDossier>
{
    public void Configure(EntityTypeBuilder<DocumentDossier> builder)
    {
        builder.ToTable("documentsDossiers");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Nom).HasMaxLength(60);
        builder.Property(d => d.Extension).HasMaxLength(12).IsRequired();

        builder.Property(d => d.FilePath).HasMaxLength(512); 

        builder.HasOne(d => d.Dossier).WithMany(dossier => dossier.DocumentsDossiers).HasForeignKey(d => d.IdDossier);
    }
}