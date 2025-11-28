using BackOffice.Application.Common.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Dossiers.Commands.RejectDossier;

public class RejectDossierCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DossierId { get; set; }

    public string Status { get; set; } = string.Empty; 
    public MotifRejetDto MotifRejet { get; set; } = new MotifRejetDto();
}