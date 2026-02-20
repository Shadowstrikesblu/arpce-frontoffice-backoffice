using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Demandes.Commands.UploadCertificat;

public class UploadCertificatCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid AttestationId { get; set; }

    public IFormFile CertificatFile { get; set; } = default!;

    public DateTime DateDelivrance { get; set; }

    public DateTime DateExpiration { get; set; }


}