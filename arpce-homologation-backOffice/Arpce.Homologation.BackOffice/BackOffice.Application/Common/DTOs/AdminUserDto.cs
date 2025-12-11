// Fichier : BackOffice.Application/Features/Authentication/Queries/CheckToken/AdminUserDto.cs

using BackOffice.Application.Features.Admin.Queries.GetAdminUsersList;
using System;

namespace BackOffice.Application.Common.DTOs;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public Guid? IdProfil { get; set; } 
    public string Compte { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Prenoms { get; set; }
    public bool ChangementMotPasse { get; set; }
    public Guid IdUtilisateurType { get; set; } 
    public AdminUserTypeSimpleDto? TypeUtilisateur { get; set; }
    public bool Desactive { get; set; }
    public string? Remarques { get; set; }
    public DateTime? DerniereConnexion { get; set; }
    public string? UtilisateurCreation { get; set; }
    public DateTime? DateCreation { get; set; }
    public string? UtilisateurModification { get; set; }
    public DateTime? DateModification { get; set; }
    public int NiveauValidation { get; set; }
    public AdminProfilDto? Profil { get; set; }
}