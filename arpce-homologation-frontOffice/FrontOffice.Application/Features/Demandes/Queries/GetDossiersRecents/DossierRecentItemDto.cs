using FrontOffice.Application.Common.DTOs;
using System;
using System.Collections.Generic;

namespace FrontOffice.Application.Features.Demandes.Queries.GetDossiersRecents;

public class DossierRecentItemDto
{
    public Guid Id { get; set; }
    public Guid IdClient { get; set; }
    public Guid IdStatut { get; set; }
    public Guid? IdModeReglement { get; set; }
    public DateTime DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public StatutDto? Statut { get; set; }
    public ICollection<DemandeDto> Demandes { get; set; } = new List<DemandeDto>();
}