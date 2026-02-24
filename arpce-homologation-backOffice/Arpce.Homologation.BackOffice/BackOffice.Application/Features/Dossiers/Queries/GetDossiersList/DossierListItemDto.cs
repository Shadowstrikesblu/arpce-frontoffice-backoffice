using System;
using System.Collections.Generic;

namespace BackOffice.Application.Common.DTOs;

public class DossierListItemDto
{
    public Guid Id { get; set; }
    public DateTime DateOuverture { get; set; }
    public DateTime? DateModification { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public ClientDto? Client { get; set; }
    public StatutDto? Statut { get; set; }

    public DemandeDto? Demande { get; set; }
    public List<DevisDto> Devis { get; set; } = new();
    public List<DocumentDossierDto> Documents { get; set; } = new();
    public List<AttestationDto> Attestations { get; set; } = new();
}