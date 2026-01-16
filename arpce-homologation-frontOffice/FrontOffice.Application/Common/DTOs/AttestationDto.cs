using System;

namespace FrontOffice.Application.Common.DTOs;

public class AttestationDto
{
    public Guid Id { get; set; }
    public long DateDelivrance { get; set; }
    public long DateExpiration { get; set; }

    public string? Extension { get; set; }
    public string? FilePath { get; set; }

}