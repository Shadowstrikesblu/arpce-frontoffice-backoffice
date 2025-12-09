using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.DTOs
{
    public class AttestationDto
    {
        public Guid Id { get; set; }
        public long DateDelivrance { get; set; }
        public long DateExpiration { get; set; }
        // On ne retourne pas les données binaires (byte[]), mais peut-être un lien de téléchargement si nécessaire.
    }
}
