using BackOffice.Application.Common.DTOs;
using System.Collections.Generic;

namespace BackOffice.Application.Features.Signataires.Queries.GetSignatairesList;

public class SignatairesListVm
{
    public List<SignataireDto> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}