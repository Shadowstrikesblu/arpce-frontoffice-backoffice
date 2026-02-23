using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Attestations.Commands.LinkSignataire;

public class LinkSignataireCommandHandler : IRequestHandler<LinkSignataireCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICertificateGeneratorService _certificateGenerator;

    public LinkSignataireCommandHandler(IApplicationDbContext context, ICertificateGeneratorService certificateGenerator)
    {
        _context = context;
        _certificateGenerator = certificateGenerator;
    }

    public async Task<bool> Handle(LinkSignataireCommand request, CancellationToken cancellationToken)
    {
        // Trouve l'attestation
        var attestation = await _context.Attestations
            .Include(a => a.Demande)
            .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

        if (attestation == null) throw new Exception("Attestation introuvable.");

        // Vérifie si le signataire existe
        var signataireExists = await _context.Signataires.AnyAsync(s => s.Id == request.SignataireId, cancellationToken);
        if (!signataireExists) throw new Exception("Signataire introuvable.");

        // Lie le signataire
        attestation.SignataireId = request.SignataireId;

        await _context.SaveChangesAsync(cancellationToken);

        // RÉGÉNÉRATION DU PDF
        // On demande au service de régénérer les documents du dossier pour inclure la signature
        await _certificateGenerator.GenerateAttestationsForDossierAsync(attestation.Demande.IdDossier);

        return true;
    }
}