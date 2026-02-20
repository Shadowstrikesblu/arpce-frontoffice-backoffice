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
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenoms { get; set; } = string.Empty;
        public string Fonction { get; set; } = string.Empty;
        public string? SignatureImagePath { get; set; } // Chemin vers le fichier sur le serveur
        public bool IsActive { get; set; } = true;
    }
}
