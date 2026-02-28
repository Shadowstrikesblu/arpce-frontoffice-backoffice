using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Dossiers.Commands.UploadReceipt;

/// <summary>
/// Commande pour téléverser le fichier du reçu signé (preuve de paiement) pour un dossier.
/// </summary>
public class UploadReceiptCommand : IRequest<bool>
{
    /// <summary>
    /// L'ID du dossier auquel le reçu est associé.
    /// </summary>
    [JsonIgnore]
    public Guid DossierId { get; set; }

    /// <summary>
    /// Le fichier PDF du reçu scanné.
    /// </summary>
    public IFormFile RecuFile { get; set; } = default!;
}