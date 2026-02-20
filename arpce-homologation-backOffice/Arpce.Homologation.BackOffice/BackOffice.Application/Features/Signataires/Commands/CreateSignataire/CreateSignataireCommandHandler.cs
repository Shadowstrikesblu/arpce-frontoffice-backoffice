using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;

namespace BackOffice.Application.Features.Signataires.Commands.CreateSignataire;

public class CreateSignataireCommandHandler : IRequestHandler<CreateSignataireCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorage; // Votre service de stockage

    public CreateSignataireCommandHandler(IApplicationDbContext context, IFileStorageProvider fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Guid> Handle(CreateSignataireCommand request, CancellationToken cancellationToken)
    {
        // 1. Sauvegarde de l'image de signature
        // On stocke dans un dossier spécifique "signatures"
        var imagePath = await _fileStorage.UploadSignatureAsync(request.SignatureFile);

        // 2. Création de l'entité
        var signataire = new Signataire
        {
            Id = Guid.NewGuid(),
            Nom = request.Nom,
            Prenoms = request.Prenoms,
            Fonction = request.Fonction,
            AdminId = request.AdminId,
            SignatureImagePath = imagePath,
            IsActive = true
        };

        _context.Signataires.Add(signataire);
        await _context.SaveChangesAsync(cancellationToken);

        return signataire.Id;
    }
}