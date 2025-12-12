using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

public class Proposition : AuditableEntity
{
    public string Code { get; set; }
    public string Libelle { get; set; }
}