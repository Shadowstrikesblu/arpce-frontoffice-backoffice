using BackOffice.Application.Features.Documents.Queries.GetFacturesList;
using System.Collections.Generic;

namespace BackOffice.Application.Common.DTOs.Documents;

public class DocumentListVm
{
    public int Page { get; set; }
    public int PageTaille { get; set; }
    public List<DocumentItemDto> Document { get; set; } = new List<DocumentItemDto>();
}