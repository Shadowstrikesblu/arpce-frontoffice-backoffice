using MediatR;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Dossiers.Commands.SendMail;

public class SendMailToClientCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DossierId { get; set; }
    public string Type { get; set; } = string.Empty; 
}