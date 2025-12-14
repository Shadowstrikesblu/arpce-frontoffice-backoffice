using BackOffice.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Commands.UpdateProfil
{
    public class UpdateProfilCommandHandler : IRequestHandler<UpdateProfilCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProfilCommandHandler(IApplicationDbContext context, IAuditService auditService, ICurrentUserService currentUserService)
        {
            _context = context;
            _auditService = auditService;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateProfilCommand request, CancellationToken cancellationToken)
        {
            // Récupération du profil
            var profil = await _context.AdminProfils.FindAsync(new object[] { request.Id }, cancellationToken);

            if (profil == null)
            {
                throw new Exception($"Profil avec l'ID '{request.Id}' introuvable.");
            }

            // Mise à jour des champs
            var oldLibelle = profil.Libelle;

            profil.Code = request.Code;
            profil.Libelle = request.Libelle;
            profil.Remarques = request.Remarques;

            // Métadonnées
            profil.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            profil.UtilisateurModification = _currentUserService.UserId?.ToString() ?? "SYSTEM";

            // Sauvegarde
            await _context.SaveChangesAsync(cancellationToken);

            // Audit
            await _auditService.LogAsync(
                page: "Gestion des Profils",
                libelle: $"Modification du profil '{oldLibelle}' (Nouveau: {request.Libelle})",
                eventTypeCode: "UPDATE"
            );

            return true;
        }
    }
}
