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
                // On récupère le signataire AVEC les infos de l'utilisateur lié
                var signataire = await _context.Signataires
                    .Include(s => s.AdminUtilisateur)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (signataire == null) return false;

                // 1. Mise à jour de la fonction (dans AdminUtilisateur)
                if (request.Fonction != null)
                {
                    signataire.AdminUtilisateur.Fonction = request.Fonction;
                }

                // 2. Mise à jour du statut actif (dans Signataire)
                if (request.IsActive.HasValue)
                {
                    signataire.IsActive = request.IsActive.Value;
                }

                // 3. Mise à jour de l'image de signature (dans Signataire)
                if (request.SignatureFile != null)
                {
                    signataire.SignatureImagePath = await _fileStorage.UploadSignatureAsync(request.SignatureFile);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }

