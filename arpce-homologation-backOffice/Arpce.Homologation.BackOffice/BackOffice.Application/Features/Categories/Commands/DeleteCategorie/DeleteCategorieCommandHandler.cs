using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.DeleteCategorie;

public class DeleteCategorieCommandHandler : IRequestHandler<DeleteCategorieCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public DeleteCategorieCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(DeleteCategorieCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoriesEquipements
            .FindAsync(new object[] { request.CategorieId }, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"La catégorie avec l'ID '{request.CategorieId}' est introuvable.");
        }

        string libelle = entity.Libelle;

        _context.CategoriesEquipements.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Gestion des Catégories",
            libelle: $"Suppression de la catégorie '{entity.Code}' (ID: {entity.Id}).",
            eventTypeCode: "SUPPRESSION");

        await _notificationService.SendToGroupAsync(
            profilCode: "ADMIN", 
            title: "Catégorie Supprimée",
            message: $"La catégorie d'équipement '{libelle}' a été supprimée.",
            type: "E"
        );

        return true;
    }
}