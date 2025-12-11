using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.DTOs
{
    public class DevisDto
    {
        public Guid Id { get; set; }
        public long Date { get; set; }
        public decimal MontantEtude { get; set; }
        public decimal? MontantHomologation { get; set; }
        public decimal? MontantControle { get; set; }
        public byte? PaiementOk { get; set; }
    }
}
