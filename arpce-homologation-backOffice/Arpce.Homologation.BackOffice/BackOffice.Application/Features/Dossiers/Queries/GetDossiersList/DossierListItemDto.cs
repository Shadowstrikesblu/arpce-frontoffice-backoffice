using BackOffice.Application.Common.DTOs;
using BackOffice.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Représente un seul dossier dans la liste paginée retournée au Back Office.
/// </summary>
public class DossierListItemDto
{
    public Guid Id { get; set; }
    public DateTime DateOuverture { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    /// <summary>
    /// Les informations sur le client qui a soumis le dossier.
    /// </summary>
    public ClientDto? Client { get; set; }

    /// <summary>
    /// Le statut actuel du dossier.
    /// </summary>
    public StatutDto? Statut { get; set; }

    /// <summary>
    /// La liste des équipements (demandes) contenus dans ce dossier.
    /// </summary>
    public List<DocumentDossierDto> Documents { get; set; } = new();
    public List<AttestationDto> Attestations { get; set; } = new();

    public List<DemandeDto> Demandes { get; set; } = new List<DemandeDto>();
}