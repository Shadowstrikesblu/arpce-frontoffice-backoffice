using MediatR;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Demandes.Commands.SetHomologable;

public class SetEquipementHomologableCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid EquipementId { get; set; }
    public bool Homologable { get; set; }
}