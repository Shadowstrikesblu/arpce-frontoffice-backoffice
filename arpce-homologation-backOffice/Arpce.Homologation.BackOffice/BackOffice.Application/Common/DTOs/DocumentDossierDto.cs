using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.DTOs
{
    public class DocumentDossierDto
    {
        public Guid Id { get; set; }
        public string? Nom { get; set; }
        public byte? Type { get; set; }
        public string? Extension { get; set; }
        public string? FilePath { get; set; }
    }
}
