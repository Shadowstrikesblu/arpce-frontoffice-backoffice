using MediatR;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Dossiers.Commands.ValidateInstruction;

/// <summary>
/// Commande pour valider l'instruction d'un dossier d'homologation.
/// Cette action met à jour le statut du dossier et ajoute un commentaire.
/// </summary>
public class ValidateInstructionCommand : IRequest<bool>
{
    /// <summary>
    /// L'identifiant du dossier dont l'instruction est validée.
    /// Provient de la route de l'URL.
    /// </summary>
    [JsonIgnore]
    public Guid DossierId { get; set; }

    /// <summary>
    /// Remarque ou commentaire de l'agent validateur.
    /// Provient du corps de la requête.
    /// </summary>
    public string? Remarque { get; set; }
}