using BackOffice.Domain.Common;
using System;

namespace BackOffice.Domain.Entities;

public class Beneficiaire : AuditableEntity
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Type { get; set; } 
    public string? Adresse { get; set; }
    public string? LettreDocumentPath { get; set; } 

    // Relation 1:1 avec la demande (équipement)
    public Guid DemandeId { get; set; }
    public virtual Demande Demande { get; set; } = default!;
}