using FrontOffice.Domain.Entities;
using FrontOffice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrontOffice.Infrastructure.Persistence.Configurations;

public class StatutConfiguration : IEntityTypeConfiguration<Statut>
{
    public void Configure(EntityTypeBuilder<Statut> builder)
    {
        builder.ToTable("statuts");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Code).HasMaxLength(120).IsRequired();
        builder.Property(s => s.Libelle).HasMaxLength(120).IsRequired();

        builder.HasData(
            new Statut { Id = Guid.NewGuid(), Code = "NouveauDossier", Libelle = "Nouvelle demande" },
            new Statut { Id = Guid.NewGuid(), Code = "Instruction", Libelle = "En cours d'instruction" },
            new Statut { Id = Guid.NewGuid(), Code = "ApprobationInstruction", Libelle = "Envoyé pour approbation" },
            new Statut { Id = Guid.NewGuid(), Code = "InstructionApprouve", Libelle = "Instruction Approuvée" },
            new Statut { Id = Guid.NewGuid(), Code = "DevisEmis", Libelle = "Devis émis" },
            new Statut { Id = Guid.NewGuid(), Code = "DevisValide", Libelle = "Devis validé par client" },
            new Statut { Id = Guid.NewGuid(), Code = "DevisRejete", Libelle = "Devis refusé par client" },
            new Statut { Id = Guid.NewGuid(), Code = "DevisPaiement", Libelle = "Approuvé, en attente de paiement" },
            new Statut { Id = Guid.NewGuid(), Code = "PaiementRejete", Libelle = "Paiement non accepté" },
            new Statut { Id = Guid.NewGuid(), Code = "PaiementExpire", Libelle = "Paiement expiré" },
            new Statut { Id = Guid.NewGuid(), Code = "DossierPaye", Libelle = "Paiement effectué" },
            new Statut { Id = Guid.NewGuid(), Code = "DossierSignature", Libelle = "Attestation en signature" },
            new Statut { Id = Guid.NewGuid(), Code = "DossierSigne", Libelle = "Attestation signée" }
        );
    }
}