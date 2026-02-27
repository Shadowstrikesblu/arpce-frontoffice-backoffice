using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
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
            FraisEtude = request.FraisEtude,
            FraisHomologation = request.FraisHomologation,
            FraisControle = request.FraisControle,
            TypeClient = request.TypeClient,
            ModeCalcul = request.ModeCalcul,
            BlockSize = request.BlockSize,
            QtyMin = request.QtyMin,
            QtyMax = request.QtyMax,
            ReferenceLoiFinance = request.ReferenceLoiFinance,

            TypeEquipement = request.TypeEquipement ?? string.Empty,
            Remarques = request.Remarques,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM"
        };

        _context.CategoriesEquipements.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new CategorieEquipementDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Libelle = entity.Libelle,
            TypeEquipement = entity.TypeEquipement,
            FraisEtude = entity.FraisEtude,
            FraisHomologation = entity.FraisHomologation,
            FraisControle = entity.FraisControle,
            ModeCalcul = entity.ModeCalcul,
            BlockSize = entity.BlockSize,
            QtyMin = entity.QtyMin,    
            QtyMax = entity.QtyMax,    
            ReferenceLoiFinance = entity.ReferenceLoiFinance 
        };
    }
}