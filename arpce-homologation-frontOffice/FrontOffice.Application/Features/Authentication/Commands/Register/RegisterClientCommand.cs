// Fichier : FrontOffice.Application/Features/Authentication/Commands/Register/RegisterClientCommand.cs

using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.Register;

public class RegisterClientCommand : IRequest<AuthenticationResult>
{
    public string RaisonSociale { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Contact
    public string ContactNom { get; set; } = string.Empty;
    public string ContactTelephone { get; set; } = string.Empty;
    public string? ContactFonction { get; set; }

    // Infos Société / Client
    public string TypeClient { get; set; } = "Entreprise"; // "Particulier" | "Entreprise"
    public string? RegistreCommerce { get; set; } 
    public string? Adresse { get; set; }
    public string? Bp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }

    // Sécurité
    public string? CaptchaToken { get; set; }
}