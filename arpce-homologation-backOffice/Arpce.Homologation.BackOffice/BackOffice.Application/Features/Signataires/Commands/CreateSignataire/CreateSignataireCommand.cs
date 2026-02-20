using MediatR;
using Microsoft.AspNetCore.Http;

namespace BackOffice.Application.Features.Signataires.Commands.CreateSignataire;

public class CreateSignataireCommand : IRequest<Guid>
{
    public string Nom { get; set; } = string.Empty;
    public string Prenoms { get; set; } = string.Empty;
    public string Fonction { get; set; } = string.Empty;
    public Guid AdminId { get; set; }
    public IFormFile SignatureFile { get; set; } = default!;
}