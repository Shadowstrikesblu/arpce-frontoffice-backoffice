using BackOffice.Application.Common.DTOs;
using System;
using System.Collections.Generic;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

public class DossierDetailVm
{
    public Guid Id { get; set; }
    public long DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public ClientDto? Client { get; set; }
    public StatutDto? Statut { get; set; }
    public ModeReglementDto? ModeReglement { get; set; }

    public DemandeDto? Demande { get; set; }

    public List<DevisDto> Devis { get; set; } = new();
    public List<CommentaireDto> Commentaires { get; set; } = new();
    public List<DocumentDossierDto> Documents { get; set; } = new();
    public List<AttestationDto> Attestations { get; set; } = new();
}