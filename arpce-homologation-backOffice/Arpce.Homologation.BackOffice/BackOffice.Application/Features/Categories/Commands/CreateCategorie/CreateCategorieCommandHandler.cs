using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Categories.Queries.GetCategoriesList;
using BackOffice.Domain.Entities;
using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.CreateCategorie;

public class CreateCategorieCommandHandler : IRequestHandler<CreateCategorieCommand, CategorieEquipementDto>
{
    private readonly IApplicationDbContext _context;

    public CreateCategorieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
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

        return new CategorieEquipementDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Libelle = entity.Libelle,
            TypeEquipement = entity.TypeEquipement,
            TypeClient = entity.TypeClient,
            FraisEtude = entity.TarifEtude,
            FraisHomologation = entity.TarifHomologation,
            FraisControle = entity.TarifControle,
            FormuleHomologation = entity.FormuleHomologation,
            QuantiteReference = entity.QuantiteReference,
            Remarques = entity.Remarques
        };
    }
}