using MediatR;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Requête MediatR pour obtenir une liste paginée, filtrée et triée des dossiers d'un client.
/// Elle utilise GetDossiersQueryParameters pour encapsuler tous les paramètres de l'URL.
/// </summary>
public class GetDossiersListQuery : IRequest<DossiersListVm>
{
    /// <summary>
    /// Les paramètres de la requête (page, taillePage, recherche, etc.).
    /// </summary>
    public GetDossiersQueryParameters Parameters { get; set; }

    /// <summary>
    /// Initialise une nouvelle instance de la requête avec les paramètres fournis.
    /// </summary>
    /// <param name="parameters">Les paramètres de recherche, tri et pagination.</param>
    public GetDossiersListQuery(GetDossiersQueryParameters parameters)
    {
        Parameters = parameters;
    }
}