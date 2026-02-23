using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace FrontOffice.Application.Features.Demandes.Commands.AddEquipementToDossier;

public class AddEquipementToDossierCommand : IRequest<bool>
{
    public Guid IdDossier { get; set; }
    public string Equipement { get; set; } = string.Empty;
    public string Modele { get; set; } = string.Empty;
    public string Marque { get; set; } = string.Empty;
    public string Fabricant { get; set; } = string.Empty;
    public string? Type { get; set; }

    public string? Description { get; set; }
    public int QuantiteEquipements { get; set; }
    public IFormFile TypeURL_FicheTechnique { get; set; } = default!;
}