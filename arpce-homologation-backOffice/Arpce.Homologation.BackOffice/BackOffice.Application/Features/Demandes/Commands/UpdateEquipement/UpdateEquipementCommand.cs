using MediatR;
using System;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Demandes.Commands.UpdateEquipement;

public class UpdateEquipementCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid EquipementId { get; set; }

    public string? Equipement { get; set; }
    public string? Modele { get; set; }
    public string? Marque { get; set; }
    public string? Fabricant { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public int? QuantiteEquipements { get; set; } // int est mieux, le front enverra un nombre
}