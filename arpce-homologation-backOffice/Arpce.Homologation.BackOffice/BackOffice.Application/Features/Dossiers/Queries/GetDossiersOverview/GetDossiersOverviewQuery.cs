using MediatR;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersOverview;

/// <summary>
/// Requête MediatR pour obtenir l'aperçu statistique global des dossiers.
/// </summary>
public class GetDossiersOverviewQuery : IRequest<DossiersOverviewDto>
{
}