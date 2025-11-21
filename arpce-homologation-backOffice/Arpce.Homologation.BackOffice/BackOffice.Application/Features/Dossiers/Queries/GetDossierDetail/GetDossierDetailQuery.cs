using MediatR;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

/// <summary>
/// Requête MediatR pour obtenir les détails complets d'un dossier spécifique.
/// </summary>
public class GetDossierDetailQuery : IRequest<DossierDetailVm>
{
    public Guid DossierId { get; set; }

    public GetDossierDetailQuery(Guid dossierId)
    {
        DossierId = dossierId;
    }
}