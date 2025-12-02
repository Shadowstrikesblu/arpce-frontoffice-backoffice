using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.CreateCategorie;

public class CreateCategorieCommandHandler : IRequestHandler<CreateCategorieCommand, CategorieEquipementDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateCategorieCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
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
            TarifEtude = request.FraisEtude,
            TarifHomologation = request.FraisHomologation,
            TarifControle = request.FraisControle,
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

        return new CategorieEquipementDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Libelle = entity.Libelle,
            TypeEquipement = entity.TypeEquipement,
            TypeClient = entity.TypeClient,
            TarifEtude = entity.TarifEtude,
            TarifHomologation = entity.TarifHomologation,
            TarifControle = entity.TarifControle,
            FormuleHomologation = entity.FormuleHomologation,
            QuantiteReference = entity.QuantiteReference,
            Remarques = entity.Remarques
        };
    }
}