using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Documents.Queries.GetFacturesList
{
    public class DossierInfoInDocDto
    {
        public string Numero { get; set; } = string.Empty;
        public string Libelle { get; set; } = string.Empty;
        public DevisInfoInDocDto? Devis { get; set; }
    }
}
