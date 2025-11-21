using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité AdminOptions.
/// Inclut le mappage de la clé primaire synthétique et des autres propriétés.
/// </summary>
public class AdminOptionsConfiguration : IEntityTypeConfiguration<AdminOptions>
{
    public void Configure(EntityTypeBuilder<AdminOptions> builder)
    {
        builder.ToTable("AdminOptions");

        builder.HasKey(ao => ao.Id); 

        builder.Property(ao => ao.JournalActivation).HasColumnType("tinyint"); 
        builder.Property(ao => ao.EventConnexion).HasColumnType("tinyint");
        builder.Property(ao => ao.EventOuverturePage).HasColumnType("tinyint");
        builder.Property(ao => ao.EventChangementMotPasse).HasColumnType("tinyint");
        builder.Property(ao => ao.EventModificationDonnees).HasColumnType("tinyint");
        builder.Property(ao => ao.EventSuppressionDonnees).HasColumnType("tinyint");
        builder.Property(ao => ao.EventImpression).HasColumnType("tinyint");
        builder.Property(ao => ao.EventValidation).HasColumnType("tinyint");
        builder.Property(ao => ao.JournalLimitation).HasColumnType("tinyint");
        builder.Property(ao => ao.JournalTypeLimitation).HasColumnType("tinyint");
        builder.Property(ao => ao.JournalDureeLimitation).HasColumnType("int"); 
        builder.Property(ao => ao.JournalTypeDureeLimitation).HasColumnType("tinyint");
        builder.Property(ao => ao.JournalTailleLimitation).HasColumnType("int");
        builder.Property(ao => ao.LDAPAuthentificationActivation).HasColumnType("tinyint");
        builder.Property(ao => ao.LDAPAuthentificationNomDomaine).HasMaxLength(60); 
        builder.Property(ao => ao.LDAPCreationAutoActivation).HasColumnType("tinyint");
        builder.Property(ao => ao.LDAPCreationAutoNomServeur).HasMaxLength(60);
        builder.Property(ao => ao.LDAPCreationAutoCompte).HasMaxLength(60);
        builder.Property(ao => ao.LDAPCreationAutoPassword).HasMaxLength(60);
        builder.Property(ao => ao.ReplicationActivation).HasColumnType("tinyint");
        builder.Property(ao => ao.ReplicationNomServeur).HasMaxLength(60);
        builder.Property(ao => ao.ReplicationCompte).HasMaxLength(60);
        builder.Property(ao => ao.ReplicationPassword).HasMaxLength(60);
    }
}