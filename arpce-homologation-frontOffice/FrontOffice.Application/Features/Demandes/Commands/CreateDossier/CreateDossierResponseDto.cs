namespace FrontOffice.Application.Features.Demandes.Commands.CreateDossier;

/// <summary>
/// Réponse lors de la création d'un dossier, retournant son identifiant.
/// </summary>
public class CreateDossierResponseDto
{
    /// <summary>
    /// L'identifiant unique du dossier nouvellement créé.
    /// </summary>
    public Guid DossierId { get; set; }
}