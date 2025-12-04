using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Dossiers.Commands.UploadFacture;

/// <summary>
/// Commande pour téléverser le fichier de la facture finale pour un dossier.
/// </summary>
public class UploadFactureCommand : IRequest<bool>
{
    /// <summary>
    /// L'ID du dossier auquel la facture est associée.
    /// </summary>
    [JsonIgnore]
    public Guid DossierId { get; set; }

    /// <summary>
    /// Le fichier PDF de la facture.
    /// </summary>
    public IFormFile FactureFile { get; set; } = default!;
}