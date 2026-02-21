using MediatR;
using System;

namespace BackOffice.Application.Features.Attestations.Commands.LinkSignataire;

public class LinkSignataireCommand : IRequest<bool>
{
    public Guid AttestationId { get; set; }
    public Guid SignataireId { get; set; }
}