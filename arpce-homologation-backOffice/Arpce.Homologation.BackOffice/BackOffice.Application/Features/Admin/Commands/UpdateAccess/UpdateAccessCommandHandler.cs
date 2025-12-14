using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.UpdateAccess
{
    public class UpdateAccessCommandHandler : IRequestHandler<UpdateAccessCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public UpdateAccessCommandHandler(IApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<bool> Handle(UpdateAccessCommand request, CancellationToken cancellationToken)
        {
            // Récupération de l'accès
            var access = await _context.AdminAccesses.FindAsync(new object[] { request.Id }, cancellationToken);

            if (access == null)
            {
                throw new Exception($"Accès avec l'ID '{request.Id}' introuvable.");
            }

            // Mise à jour des champs
            access.Libelle = request.Libelle;
            access.Groupe = request.Groupe;
            access.Application = request.Application;
            access.Page = request.Page;
            access.Type = request.Type;
            access.Code = request.Code;
            access.Inactif = request.Inactif ? (byte)1 : (byte)0;

            // Mise à jour des flags de permission autorisés pour cet écran
            access.Ajouter = request.Ajouter ? (byte)1 : (byte)0;
            access.Valider = request.Valider ? (byte)1 : (byte)0;
            access.Supprimer = request.Supprimer ? (byte)1 : (byte)0;
            access.Imprimer = request.Imprimer ? (byte)1 : (byte)0;

            // Sauvegarde
            await _context.SaveChangesAsync(cancellationToken);

            // Audit
            await _auditService.LogAsync(
                page: "Gestion des Accès",
                libelle: $"Modification de l'accès '{request.Libelle}' (Groupe: {request.Groupe})",
                eventTypeCode: "UPDATE"
            );

            return true;
        }
    }
}
