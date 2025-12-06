using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Commands.UploadPreuvePaiement;

public class UploadPreuvePaiementCommandHandler : IRequestHandler<UploadPreuvePaiementCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageProvider _fileStorageProvider;

    public UploadPreuvePaiementCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IFileStorageProvider fileStorageProvider)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageProvider = fileStorageProvider;
    }

    public async Task<bool> Handle(UploadPreuvePaiementCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        // ... valider user et dossier ...

        var dossier = await _context.Dossiers.FirstOrDefaultAsync(d => d.Id == request.DossierId && d.IdClient == userId, cancellationToken);
        if (dossier == null) throw new Exception("Dossier introuvable.");

        // Utiliser le service de stockage pour importer la preuve
        // On utilise un type spécifique (ex: 3) pour "Preuve de Paiement"
        await _fileStorageProvider.ImportDocumentDossierAsync(
            file: request.PreuvePaiement,
            nom: $"PreuvePaiement_{dossier.Numero}",
            type: 3,
            dossierId: dossier.Id
        );

        // Optionnel : Changer le statut du dossier à "En attente de validation de paiement"
        // var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.QuelqueChose, cancellationToken);
        // dossier.IdStatut = nouveauStatut.Id;
        // await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}