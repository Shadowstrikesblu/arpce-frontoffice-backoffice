using BackOffice.Application.Common.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Demandes.Commands.RejectEquipement;

public class RejectEquipementCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid EquipementId { get; set; }

    public string Status { get; set; } = string.Empty;
    public MotifRejetDto MotifRejet { get; set; } = new MotifRejetDto();
}