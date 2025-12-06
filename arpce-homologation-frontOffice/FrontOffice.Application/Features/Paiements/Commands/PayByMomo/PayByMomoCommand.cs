using MediatR;
using System.Text.Json.Serialization;

namespace FrontOffice.Application.Features.Paiements.Commands.PayByMomo;

public class PayByMomoCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DossierId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Provider { get; set; } = "mtn"; // 'mtn' | 'airtel'
}