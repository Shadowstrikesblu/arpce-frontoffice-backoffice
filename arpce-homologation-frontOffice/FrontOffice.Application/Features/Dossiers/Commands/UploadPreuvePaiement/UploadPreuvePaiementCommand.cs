using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace FrontOffice.Application.Features.Dossiers.Commands.UploadPreuvePaiement;

public class UploadPreuvePaiementCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DossierId { get; set; }
    public IFormFile PreuvePaiement { get; set; } = default!;
}