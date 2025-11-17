using MediatR;
using Microsoft.AspNetCore.Http; 
namespace FrontOffice.Application.Features.Demandes.Commands.CreateDossier;

/// <summary>
/// Commande pour créer un nouveau dossier d'homologation.
/// </summary>
public class CreateDossierCommand : IRequest<CreateDossierResponseDto>
{
    /// <summary>
    /// Le nom ou libellé du dossier. Doit être unique.
    /// </summary>
    public string Libelle { get; set; } = string.Empty;

    /// <summary>
    /// La lettre de demande d'homologation (fichier PDF, max 3 Mo).
    /// </summary>
    public IFormFile CourrierFile { get; set; } = default!; 
}