using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Documents.Queries.GetFacturesList
{
    public class DocumentItemDto
    {
        public string Nom { get; set; } = string.Empty;
        public int Type { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public DossierInfoInDocDto Dossier { get; set; } = new();
    }
}
