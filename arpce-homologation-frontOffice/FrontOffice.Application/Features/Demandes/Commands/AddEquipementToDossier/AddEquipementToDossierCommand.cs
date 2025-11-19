using MediatR;
using Microsoft.AspNetCore.Http; 
using System;

namespace FrontOffice.Application.Features.Demandes.Commands.AddEquipementToDossier;

/// <summary>
/// Commande pour ajouter un équipement et sa fiche technique à un dossier d'homologation existant.
/// Cette commande mappe directement les champs envoyés via un formulaire multipart/form-data.
/// </summary>
public class AddEquipementToDossierCommand : IRequest<bool> 
{
    /// <summary>
    /// L'identifiant unique (Guid) du dossier auquel cet équipement doit être rattaché.
    /// </summary>
    public Guid IdDossier { get; set; }

    /// <summary>
    /// Le nom générique de l'équipement (ex: "Imprimante Laser").
    /// </summary>
    public string Equipement { get; set; } = string.Empty;

    /// <summary>
    /// Le modèle spécifique de l'équipement (ex: "HP LaserJet Pro M40").
    /// </summary>
    public string Modele { get; set; } = string.Empty;

    /// <summary>
    /// La marque de l'équipement (ex: "HP").
    /// </summary>
    public string Marque { get; set; } = string.Empty;

    /// <summary>
    /// Le fabricant de l'équipement (ex: "Hewlett-Packard").
    /// </summary>
    public string Fabricant { get; set; } = string.Empty;

    /// <summary>
    /// Une description technique ou fonctionnelle de l'équipement.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// La quantité d'équipements de ce type concernés par la demande.
    /// </summary>
    public int QuantiteEquipements { get; set; }

    /// <summary>
    /// Le nom complet de la personne contact pour cet équipement.
    /// </summary>
    public string? ContactNom { get; set; }

    /// <summary>
    /// L'adresse e-mail de la personne contact pour cet équipement.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Le numéro de téléphone de la personne contact pour cet équipement.
    /// </summary>
    public string? ContactTelephone { get; set; }

    /// <summary>
    /// La fonction de la personne contact pour cet équipement.
    /// </summary>
    public string? ContactFonction { get; set; }

    /// <summary>
    /// Le fichier de la fiche technique de l'équipement (ex: un PDF).
    /// Le nom de la propriété correspond à ce que le frontend enverra ("TypeURL_FicheTechnique").
    /// </summary>
    public IFormFile TypeURL_FicheTechnique { get; set; } = default!;
}