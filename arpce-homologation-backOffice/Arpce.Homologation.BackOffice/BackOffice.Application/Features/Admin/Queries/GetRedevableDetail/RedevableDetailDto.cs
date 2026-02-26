using BackOffice.Application.Features.Admin.Queries.GetRedevableDetail;
using System;
using System.Collections.Generic;

namespace BackOffice.Application.Common.DTOs;

public class RedevableDetailDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string RaisonSociale { get; set; } = string.Empty;
    public string? RegistreCommerce { get; set; }
    public bool Desactive { get; set; } 
    public string? ContactNom { get; set; }
    public string? ContactTelephone { get; set; }
    public string? ContactFonction { get; set; }
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public string? Bp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public string? Remarques { get; set; }
    public string? UtilisateurCreation { get; set; }
    public int NiveauValidation { get; set; }
    public DateTime DateCreation { get; set; }
    public string? UtilisateurModification { get; set; }
    public DateTime? DateModification { get; set; }

    public List<DossierRedevableDto> Dossiers { get; set; } = new();
}

