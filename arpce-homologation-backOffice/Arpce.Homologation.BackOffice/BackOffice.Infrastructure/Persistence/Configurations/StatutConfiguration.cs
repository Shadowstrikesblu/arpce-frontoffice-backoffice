using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Statut.
/// Mappe la classe .NET à la table 'statuts'.
/// Inclut le seed de données initiales pour les statuts de dossier.
/// </summary>
public class StatutConfiguration : IEntityTypeConfiguration<Statut>
{
    public void Configure(EntityTypeBuilder<Statut> builder)
    {
        builder.ToTable("statuts");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .HasMaxLength(120) 
            .IsRequired();

        builder.Property(s => s.Libelle)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasData(
    new Statut { Id = Guid.Parse("9F1C2F69-5D8E-4EC8-A6A1-0AA1E1C5A201"), Code = "NouveauDossier", Libelle = "Nouvelle demande" },
    new Statut { Id = Guid.Parse("A7C55954-7B1C-4F43-9CC4-1F2AF3CCA202"), Code = "RefusDossier", Libelle = "Refus de la demande" },
    new Statut { Id = Guid.Parse("3B9ED3A1-1E24-4D0C-8F13-7C55C9BAA203"), Code = "Instruction", Libelle = "En cours d'instruction" },
    new Statut { Id = Guid.Parse("0D4F6E52-AD8E-4B67-9A11-985B66DCA204"), Code = "ApprobationInstruction", Libelle = "Envoyé pour approbation" },
    new Statut { Id = Guid.Parse("84C6D32B-1F44-4FBD-8C91-12E3B5E0A205"), Code = "InstructionApprouve", Libelle = "Instruction Approuvée" },
    new Statut { Id = Guid.Parse("D62E63CB-4C2F-4E24-B5A2-8FAE11E0A206"), Code = "DevisCreer", Libelle = "Devis créé" },
    new Statut { Id = Guid.Parse("AE906D70-A1C2-4B2A-8DB7-B22C6D4CA207"), Code = "DevisValideSC", Libelle = "Devis validé par Chef Service" },
    new Statut { Id = Guid.Parse("FC01B3E8-82D8-4E55-953F-0FC9EDB2A208"), Code = "DevisValideTr", Libelle = "Devis validé par Trésorerie" },
    new Statut { Id = Guid.Parse("CCF4F5B7-8BE7-4F01-9C09-FA5522D6A209"), Code = "DevisEmit", Libelle = "Devis émis" },
    new Statut { Id = Guid.Parse("8C2BC784-06A3-4D73-A9F4-5F6D94A7A210"), Code = "DevisValide", Libelle = "Devis validé par client" },
    new Statut { Id = Guid.Parse("CD3C7E21-6909-4A29-9F0E-90E9AC2DA211"), Code = "DevisRefuser", Libelle = "Devis refusé par client" },
    new Statut { Id = Guid.Parse("1A173AB7-6DB7-4B9F-906E-A85352A4A212"), Code = "DevisPaiement", Libelle = "En attente de paiement" },
    new Statut { Id = Guid.Parse("0C47F66C-3AE3-4DBF-AF5B-1B7EB3F2A213"), Code = "PaiementRejete", Libelle = "Paiement non accepté" },
    new Statut { Id = Guid.Parse("DF4BB5AA-17D5-4C04-8545-48F59C59A214"), Code = "PaiementExpirer", Libelle = "Paiement expiré" },
    new Statut { Id = Guid.Parse("33B7BD1D-5901-4AFE-BE70-D4C10E3FA215"), Code = "DossierPayer", Libelle = "Paiement effectué" },
    new Statut { Id = Guid.Parse("0E9A8BB4-7989-4EB8-9F21-4F9B7FFC A216"), Code = "DossierSignature", Libelle = "Attestation en signature" },
    new Statut { Id = Guid.Parse("ED13C54B-5E63-4A0F-A0A7-332A7C27A217"), Code = "DossierSigner", Libelle = "Attestation signée" }
);

    }
}