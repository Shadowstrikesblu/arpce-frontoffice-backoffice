using MediatR;
using System;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

/// <summary>
/// Requête MediatR pour obtenir les détails complets d'un dossier spécifique.
/// </summary>
public class GetDossierDetailQuery : IRequest<DossierDetailVm>
{
    /// <summary>
    /// L'identifiant unique (Guid) du dossier à récupérer.
    /// </summary>
    public Guid DossierId { get; set; }

    public GetDossierDetailQuery(Guid dossierId)
    {
        DossierId = dossierId;
    }
}