using FrontOffice.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Domain.Entities
{
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
}
