using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Signataires.Commands.UpdateSignataire
{
    public class UpdateSignataireCommandHandler : IRequestHandler<UpdateSignataireCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageProvider _fileStorage;

        public UpdateSignataireCommandHandler(IApplicationDbContext context, IFileStorageProvider fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<bool> Handle(UpdateSignataireCommand request, CancellationToken cancellationToken)
        {
            var signataire = await _context.Signataires.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (signataire == null) return false;

            if (request.Nom != null) signataire.Nom = request.Nom;
            if (request.Prenoms != null) signataire.Prenoms = request.Prenoms;
            if (request.Fonction != null) signataire.Fonction = request.Fonction;
            if (request.IsActive.HasValue) signataire.IsActive = request.IsActive.Value;

            // Si une nouvelle signature est fournie, on remplace l'ancienne
            if (request.SignatureFile != null)
            {
                signataire.SignatureImagePath = await _fileStorage.UploadSignatureAsync(request.SignatureFile);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
