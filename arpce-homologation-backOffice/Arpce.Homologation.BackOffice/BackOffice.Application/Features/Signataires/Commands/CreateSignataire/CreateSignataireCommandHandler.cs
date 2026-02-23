using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        // On vérifie si l'utilisateur existe
        var user = await _context.AdminUtilisateurs.FirstOrDefaultAsync(u => u.Id == request.AdminId, cancellationToken);
        if (user == null) throw new Exception("Utilisateur introuvable.");

        // Gestion de l'image (Optionnelle)
        string? path = null;
        if (request.SignatureFile != null)
        {
            path = await _fileStorage.UploadSignatureAsync(request.SignatureFile);
        }

        // Création du lien Signataire
        var signataire = new Signataire
        {
            Id = request.AdminId, 
            SignatureImagePath = path,
            IsActive = true
        };

        _context.Signataires.Add(signataire);
        await _context.SaveChangesAsync(cancellationToken);

        return signataire.Id;
    }
}