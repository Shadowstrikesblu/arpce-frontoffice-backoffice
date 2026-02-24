using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Attestations.Commands.LinkSignataire;

public class LinkSignataireCommandHandler : IRequestHandler<LinkSignataireCommand, AttestationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICertificateGeneratorService _certificateGenerator;

    public LinkSignataireCommandHandler(
        IApplicationDbContext context,
        ICertificateGeneratorService certificateGenerator)
    {
        _context = context;
        _certificateGenerator = certificateGenerator;
    }

    public async Task<AttestationDto> Handle(LinkSignataireCommand request, CancellationToken cancellationToken)
    {
        var attestation = await _context.Attestations
            .Include(a => a.Demande)
            .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

        if (attestation == null) throw new Exception("Attestation introuvable.");

        var signataireExists = await _context.Signataires.AnyAsync(s => s.Id == request.SignataireId, cancellationToken);
        if (!signataireExists) throw new Exception("Signataire introuvable.");

        attestation.SignataireId = request.SignataireId;
        await _context.SaveChangesAsync(cancellationToken);

        await _certificateGenerator.GenerateAttestationsForDossierAsync(attestation.Demande.IdDossier);

        var updatedAttestation = await _context.Attestations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

        return new AttestationDto
        {
            Id = updatedAttestation!.Id,
            DateDelivrance = updatedAttestation.DateDelivrance,
            DateExpiration = updatedAttestation.DateExpiration,
            FilePath = $"/api/documents/attestation/{updatedAttestation.Id}/download"
        };
    }
}