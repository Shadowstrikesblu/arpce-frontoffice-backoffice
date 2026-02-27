using FrontOffice.Application.Common.DTOs;
using System;
using System.Collections.Generic;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersFullList;

/// <summary>
/// DTO représentant un dossier avec toutes ses informations et relations imbriquées.
/// </summary>
public class DossierFullListItemDto
{
    public Guid Id { get; set; }
    public long DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    public StatutDto? Statut { get; set; }
    public ModeReglementDto? ModeReglement { get; set; }

    //public int NbDemandes { get; set; }

    public List<DemandeDto> Demandes { get; set; } = new List<DemandeDto>();
    public List<DevisDto> Devis { get; set; } = new List<DevisDto>();
    public List<CommentaireDto> Commentaires { get; set; } = new List<CommentaireDto>();
    public List<DocumentDossierDto> Documents { get; set; } = new List<DocumentDossierDto>();
    public List<AttestationDto> Attestations { get; set; } = new List<AttestationDto>();
}