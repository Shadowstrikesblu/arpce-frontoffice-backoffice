using BackOffice.Application.Common.DTOs; 
using MediatR;

namespace BackOffice.Application.Features.Status.Queries.GetStatusList;

/// <summary>
/// Requête MediatR pour obtenir la liste complète de tous les statuts disponibles.
/// </summary>
public class GetStatusListQuery : IRequest<List<StatutDto>>
{
}