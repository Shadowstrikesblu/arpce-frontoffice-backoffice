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
             new Statut { Id = Guid.NewGuid(), Code = "RefusDossier", Libelle = "Refus de la demande" },
             new Statut { Id = Guid.NewGuid(), Code = "Instruction", Libelle = "En cours d'instruction" },
             new Statut { Id = Guid.NewGuid(), Code = "ApprobationInstruction", Libelle = "Envoyé pour approbation" },
             new Statut { Id = Guid.NewGuid(), Code = "InstructionApprouve", Libelle = "Instruction Approuvée" },
             new Statut { Id = Guid.NewGuid(), Code = "DevisCreer", Libelle = "Devis créé" },
             new Statut { Id = Guid.NewGuid(), Code = "DevisValideSC", Libelle = "Devis validé par Chef Service" },
             new Statut { Id = Guid.NewGuid(), Code = "DevisValideTr", Libelle = "Devis validé par Trésorerie" },
             new Statut { Id = Guid.NewGuid(), Code = "DevisEmit", Libelle = "Devis émis" },
             new Statut { Id = Guid.NewGuid(), Code = "DevisValide", Libelle = "Devis validé par client" },
             new Statut { Id = Guid.NewGuid(), Code = "DevisRefuser", Libelle = "Devis refusé par client" },
             new Statut { Id = Guid.NewGuid(), Code = "DevisPaiement", Libelle = "En attente de paiement" },
             new Statut { Id = Guid.NewGuid(), Code = "PaiementRejete", Libelle = "Paiement non accepté" },
             new Statut { Id = Guid.NewGuid(), Code = "PaiementExpirer", Libelle = "Paiement expiré" },
             new Statut { Id = Guid.NewGuid(), Code = "DossierPayer", Libelle = "Paiement effectué" },
             new Statut { Id = Guid.NewGuid(), Code = "DossierSignature", Libelle = "Attestation en signature" },
             new Statut { Id = Guid.NewGuid(), Code = "DossierSigner", Libelle = "Attestation signée" }
         );
    }
}