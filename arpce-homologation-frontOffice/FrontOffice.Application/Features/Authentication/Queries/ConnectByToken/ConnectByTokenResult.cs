using System;

namespace FrontOffice.Application.Features.Authentication.Queries.ConnectByToken;

public class ConnectByTokenResult
{
    public string Message { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RaisonSociale { get; set; } = string.Empty;
    public string? CodeClient { get; set; }
    public string? RegistreCommerce { get; set; }
    public string? ContactNom { get; set; }
    public string? ContactTelephone { get; set; }
    public string? ContactFonction { get; set; }
    public string? Adresse { get; set; }
    public string? Bp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public bool IsVerified { get; set; }
    public int NiveauValidation { get; set; }
}