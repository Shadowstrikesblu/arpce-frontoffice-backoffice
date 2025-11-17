using System;

namespace FrontOffice.Application.Common.DTOs;

public class DocumentDossierDto
{
    public Guid Id { get; set; }
    public string? Nom { get; set; }
    public byte? Type { get; set; } 
    public string? Extension { get; set; }
    public string? FilePath { get; set; } 
}