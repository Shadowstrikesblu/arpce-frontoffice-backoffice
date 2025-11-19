using System;

namespace FrontOffice.Application.Common.DTOs;

/// <summary>
/// DTO représentant les informations de contact d'un client.
/// </summary>
public class ClientContactDto
{
    /// <summary>
    /// L'identifiant unique du client.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Le nom complet de la personne contact.
    /// </summary>
    public string? ContactNom { get; set; }

    /// <summary>
    /// Le numéro de téléphone de la personne contact.
    /// </summary>
    public string? ContactTelephone { get; set; }

    /// <summary>
    /// La fonction de la personne contact.
    /// </summary>
    public string? ContactFonction { get; set; }
}