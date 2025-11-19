using FrontOffice.Application.Common.DTOs;
using MediatR;
using System;
using System.Text.Json.Serialization; 

namespace FrontOffice.Application.Features.Clients.Commands.UpdateClientContact;

/// <summary>
/// Commande pour mettre à jour les informations de contact d'un client.
/// </summary>
public class UpdateClientContactCommand : IRequest<ClientContactDto>
{
    /// <summary>
    /// L'identifiant du client à mettre à jour.
    /// </summary>
    [JsonIgnore] // Empêche ce champ d'être attendu dans le corps JSON de la requête
    public Guid ClientId { get; set; }

    /// <summary>
    /// Le nouveau nom complet de la personne contact.
    /// Si null, ce champ ne sera pas mis à jour.
    /// </summary>
    public string? ContactNom { get; set; }

    /// <summary>
    /// Le nouveau numéro de téléphone de la personne contact.
    /// </summary>
    public string? ContactTelephone { get; set; }

    /// <summary>
    /// La nouvelle fonction de la personne contact.
    /// </summary>
    public string? ContactFonction { get; set; }
}