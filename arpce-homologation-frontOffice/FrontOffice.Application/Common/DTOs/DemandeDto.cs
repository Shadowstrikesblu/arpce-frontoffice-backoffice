using System;
using System.Collections.Generic;

namespace FrontOffice.Application.Common.DTOs;

public class DemandeDto
{
    public Guid Id { get; set; }
    public Guid IdDossier { get; set; }
    //public string? NumeroDemande { get; set; }
    public string? Equipement { get; set; }
    public string? Modele { get; set; }
    public string? Marque { get; set; }
    public string? Fabricant { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public int? QuantiteEquipements { get; set; }
    public string? ContactNom { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactTelephone { get; set; }

    public decimal? PrixUnitaire { get; set; }
    //public decimal? Remise { get; set; }
    public bool EstHomologable { get; set; }

    public bool RequiertEchantillon { get; set; }
    public bool EchantillonSoumis { get; set; }
    public BeneficiaireDto? Beneficiaire { get; set; }

    public StatutDto? Statut { get; set; }
    public CategorieEquipementDto? CategorieEquipement { get; set; }
    public MotifRejetDto? MotifRejet { get; set; }
    public PropositionDto? Proposition { get; set; }

    public List<DocumentDossierDto> Documents { get; set; } = new();
}