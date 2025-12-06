using BackOffice.Application.Features.Admin.Queries.GetRedevablesList; 
using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetRedevablesAValider;

/// <summary>
/// Requête pour obtenir la liste paginée des redevables qui sont en attente de validation
/// administrative (Niveau de validation = 1).
/// </summary>
public class GetRedevablesAValiderQuery : IRequest<RedevableListVm>
{
    public int Page { get; set; } = 1;
    public int PageTaille { get; set; } = 10;
    public string? Recherche { get; set; }
    public string? Ordre { get; set; } // 'asc' | 'desc'
}