using FrontOffice.Application.Common.DTOs;
using MediatR;

namespace FrontOffice.Application.Features.Clients.Commands.UpdateClientProfile;
public class UpdateClientProfileCommand : IRequest<ClientProfileDto>
{
    public string? RaisonSociale { get; set; }
    public string? RegistreCommerce { get; set; }
    public string? ContactNom { get; set; }
    public string? ContactTelephone { get; set; }
    public string? ContactFonction { get; set; }
    public string? Adresse { get; set; }
    public string? Bp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    // Pas de 'Remarques' dans le nouveau schéma du PDF
}