using FrontOffice.Application.Common.DTOs;
using System;
using System.Collections.Generic;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

public class DossierListItemDto
{
    public Guid Id { get; set; }
    public long DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public StatutDto? Statut { get; set; }
    public List<DocumentDossierDto> Documents { get; set; } = new();
    public List<DevisDto> Devis { get; set; } = new();
    public List<AttestationDto> Attestations { get; set; } = new();
    public List<DemandeDto> Demandes { get; set; } = new();
}