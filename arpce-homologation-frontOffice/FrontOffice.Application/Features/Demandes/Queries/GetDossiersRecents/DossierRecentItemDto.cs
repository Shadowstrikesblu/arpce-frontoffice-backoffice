using FrontOffice.Application.Common.DTOs; 

namespace FrontOffice.Application.Features.Demandes.Queries.GetDossiersRecents;

public class DossierRecentItemDto
{
    public Guid Id { get; set; }
    public Guid IdClient { get; set; }
    public Guid IdStatut { get; set; }
    public Guid IdModeReglement { get; set; }
    public DateTime DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    // Utilisation du DTO pour éviter les références circulaires
    public StatutDto? Statut { get; set; }

    // Utilisation du DTO pour éviter les références circulaires
    public ICollection<DemandeDto> Demandes { get; set; } = new List<DemandeDto>();
}