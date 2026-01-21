using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.CreateCategorie;

public class CreateCategorieCommandHandler : IRequestHandler<CreateCategorieCommand, CategorieEquipementDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public CreateCategorieCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<CategorieEquipementDto> Handle(CreateCategorieCommand request, CancellationToken cancellationToken)
    {
        var entity = new CategorieEquipement
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Libelle = request.Libelle,
            TypeEquipement = request.TypeEquipement,
            TypeClient = request.TypeClient,
            FraisEtude = request.FraisEtude,
            FraisHomologation = request.FraisHomologation,
            FraisControle = request.FraisControle,
            FormuleHomologation = request.FormuleHomologation,
            QuantiteReference = request.QuantiteReference,
            Remarques = request.Remarques
        };

        _context.CategoriesEquipements.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Gestion des Catégories",
            libelle: $"Création de la catégorie '{request.Code}' - {request.Libelle}.",
            eventTypeCode: "CREATION");

        await _notificationService.SendToGroupAsync(
            profilCode: "ADMIN", 
            title: "Nouvelle Catégorie",
            message: $"La catégorie d'équipement '{entity.Libelle}' a été créée.",
            type: "E"
        );

        return new CategorieEquipementDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Libelle = entity.Libelle,
            TypeEquipement = entity.TypeEquipement,
            TypeClient = entity.TypeClient,
            FraisEtude = entity.FraisEtude,
            FraisHomologation = entity.FraisHomologation,
            FraisControle = entity.FraisControle,
            FormuleHomologation = entity.FormuleHomologation,
            QuantiteReference = entity.QuantiteReference,
            Remarques = entity.Remarques
        };
    }
}