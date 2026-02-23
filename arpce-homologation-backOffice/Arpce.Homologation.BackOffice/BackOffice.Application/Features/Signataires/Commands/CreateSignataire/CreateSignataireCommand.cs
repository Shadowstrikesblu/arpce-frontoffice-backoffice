using MediatR;
using Microsoft.AspNetCore.Http;

namespace BackOffice.Application.Features.Signataires.Commands.CreateSignataire;

public class CreateSignataireCommand : IRequest<Guid>
{
    public Guid AdminId { get; set; } 
    public IFormFile? SignatureFile { get; set; } 
}