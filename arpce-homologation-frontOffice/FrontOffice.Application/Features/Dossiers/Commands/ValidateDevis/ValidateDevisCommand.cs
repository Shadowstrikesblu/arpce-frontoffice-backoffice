using MediatR;
using System.Text.Json.Serialization;

namespace FrontOffice.Application.Features.Dossiers.Commands.ValidateDevis;

public class ValidateDevisCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DossierId { get; set; }
}