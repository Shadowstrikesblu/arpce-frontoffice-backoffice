using MediatR;
using System;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Demandes.Commands.ChangeStatus;

public class ChangeEquipementStatusCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid EquipementId { get; set; }
    public string CodeStatut { get; set; } = string.Empty; 
    public string? Commentaire { get; set; }
}