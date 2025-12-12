using FrontOffice.Application.Common.DTOs; 
using System;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Représente un seul dossier dans la liste paginée des dossiers.
/// </summary>
public class DossierListItemDto
{
    /// <summary>
    /// L'identifiant unique du dossier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// La date de création du dossier.
    /// </summary>
    public long DateOuverture { get; set; }

    /// <summary>
    /// Le numéro unique du dossier.
    /// </summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Le libellé ou nom du dossier.
    /// </summary>
    public string Libelle { get; set; } = string.Empty;

    /// <summary>
    /// L'objet Statut associé au dossier.
    /// </summary>
    public StatutDto? Statut { get; set; }
    public List<DocumentDossierDto> Documents { get; set; } = new List<DocumentDossierDto>();
}