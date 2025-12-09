using BackOffice.Application.Common.DTOs;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

/// <summary>
/// ViewModel contenant toutes les informations détaillées d'un dossier d'homologation pour le Back Office.
/// </summary>
public class DossierDetailVm
{
    public Guid Id { get; set; }
    public long DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    public ClientDto? Client { get; set; } 
    public StatutDto? Statut { get; set; }
    public ModeReglementDto? ModeReglement { get; set; }
    public List<DemandeDto> Demandes { get; set; } = new List<DemandeDto>();
    public List<DevisDto> Devis { get; set; } = new List<DevisDto>();
    public List<CommentaireDto> Commentaires { get; set; } = new List<CommentaireDto>();
    public List<DocumentDossierDto> Documents { get; set; } = new List<DocumentDossierDto>();
    public List<AttestationDto> Attestations { get; set; } = new List<AttestationDto>();
}