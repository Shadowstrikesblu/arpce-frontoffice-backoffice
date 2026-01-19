using FrontOffice.Application.Common.DTOs; 
using System;
using System.Collections.Generic;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

/// <summary>
/// ViewModel contenant toutes les informations détaillées d'un dossier d'homologation.
/// </summary>
public class DossierDetailVm
{
    public Guid Id { get; set; }
    public long DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    /// <summary>
    /// Le statut actuel du dossier.
    /// </summary>
    public StatutDto? Statut { get; set; }

    /// <summary>
    /// Le mode de règlement choisi pour le dossier.
    /// </summary>
    public ModeReglementDto? ModeReglement { get; set; }

    /// <summary>
    /// La liste des équipements (demandes) associés à ce dossier.
    /// </summary>
    public List<DemandeDto> Demandes { get; set; } = new List<DemandeDto>();

    /// <summary>
    /// La liste des devis générés pour ce dossier.
    /// </summary>
    public List<DevisDto> Devis { get; set; } = new List<DevisDto>();

    /// <summary>
    /// La liste des commentaires échangés sur ce dossier.
    /// </summary>
    public List<CommentaireDto> Commentaires { get; set; } = new List<CommentaireDto>();

    /// <summary>
    /// La liste des documents attachés au dossier 
    /// </summary>
    public List<DocumentDossierDto> Documents { get; set; } = new List<DocumentDossierDto>();

    /// <summary>
    /// La liste des attestations générées pour les équipements de ce dossier.
    /// </summary>
    public List<AttestationDto> Attestations { get; set; } = new List<AttestationDto>();

}