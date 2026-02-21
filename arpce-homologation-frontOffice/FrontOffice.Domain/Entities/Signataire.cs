using FrontOffice.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Domain.Entities
{
    public class Signataire : AuditableEntity
    {
        // L'Id ici sera le même que celui de l'AdminUtilisateur
        public Guid Id { get; set; }

        // Propriété spécifique uniquement au signataire
        public string? SignatureImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation vers l'utilisateur complet
        public virtual AdminUtilisateur AdminUtilisateur { get; set; } = default!;
    }
}
