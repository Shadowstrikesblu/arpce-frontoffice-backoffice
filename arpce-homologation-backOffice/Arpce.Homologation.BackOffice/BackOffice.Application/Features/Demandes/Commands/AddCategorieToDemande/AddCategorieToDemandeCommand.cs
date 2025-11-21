// Fichier : BackOffice.Application/Features/Demandes/Commands/AddCategorieToDemande/AddCategorieToDemandeCommand.cs

using MediatR;
using System;
using System.Text.Json.Serialization; // Pour [JsonIgnore]

namespace BackOffice.Application.Features.Demandes.Commands.AddCategorieToDemande;

/// <summary>
/// Commande pour assigner une catégorie à une demande (équipement) existante.
/// </summary>
public class AddCategorieToDemandeCommand : IRequest<bool> 
{
   /// <summary>
   ///  L'identifiant de la demande (équipement) à mettre à jour.
    //Cette valeur viendra de la route de l'URL.
   /// </summary>
    [JsonIgnore]
    public Guid DemandeId { get; set; }

   /// <summary>
   /// L'identifiant de la catégorie à assigner.
   /// </summary>
    public Guid CategorieId { get; set; }
}