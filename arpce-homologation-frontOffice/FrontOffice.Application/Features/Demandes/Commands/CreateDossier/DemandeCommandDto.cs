using FrontOffice.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Demandes.Commands.CreateDossier
{
    public class DemandeCommandDto
    {
        public string Equipement { get; set; } = string.Empty;
        public string Modele { get; set; } = string.Empty;
        public string Marque { get; set; } = string.Empty;
        public string Fabricant { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? Description { get; set; }
        public int QuantiteEquipements { get; set; }
        public decimal? PrixUnitaire { get; set; } 

        public bool RequiertEchantillon { get; set; }

        public BeneficiaireDto? Beneficiaire { get; set; }
    }
}
