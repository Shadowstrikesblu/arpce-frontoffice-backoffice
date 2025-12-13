using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CommentaireConfiguration : IEntityTypeConfiguration<Commentaire>
{
    public void Configure(EntityTypeBuilder<Commentaire> builder)
    {
        builder.ToTable("commentaires");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.DateCommentaire).HasColumnType("bigint").IsRequired();
        builder.Property(c => c.CommentaireTexte).HasColumnName("commentaire").HasMaxLength(512);
        builder.Property(c => c.NomInstructeur).HasMaxLength(60);
        builder.HasOne(c => c.Dossier).WithMany(d => d.Commentaires).HasForeignKey(c => c.IdDossier);
    }
}