using MediatR;
using System;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Demandes.Commands.AddCategorieToDemande;

/// <summary>
/// Commande pour assigner ou dissocier une catégorie à une demande (équipement).
/// </summary>
public class AddCategorieToDemandeCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DemandeId { get; set; }

    /// <summary>
    /// Si NULL, la catégorie sera dissociée de l'équipement.
    /// </summary>
    public Guid? CategorieId { get; set; }
}