using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore; 

namespace BackOffice.Application.Features.Categories.Commands.UpdateCategorie;

public class UpdateCategorieCommandHandler : IRequestHandler<UpdateCategorieCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateCategorieCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(UpdateCategorieCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoriesEquipements
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"La catégorie avec l'ID '{request.Id}' est introuvable.");
        }

        if (request.Code != null) entity.Code = request.Code;
        if (request.Libelle != null) entity.Libelle = request.Libelle;
        if (request.TypeEquipement != null) entity.TypeEquipement = request.TypeEquipement;
        if (request.TypeClient != null) entity.TypeClient = request.TypeClient;
        if (request.FraisEtude.HasValue) entity.TarifEtude = request.FraisEtude;
        if (request.FraisHomologation.HasValue) entity.TarifHomologation = request.FraisHomologation;
        if (request.FraisControle.HasValue) entity.TarifControle = request.FraisControle;
        if (request.FormuleHomologation != null) entity.FormuleHomologation = request.FormuleHomologation;
        if (request.QuantiteReference.HasValue) entity.QuantiteReference = request.QuantiteReference;
        if (request.Remarques != null) entity.Remarques = request.Remarques;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion des Catégories",
    libelle: $"Modification de la catégorie '{entity.Code}' (ID: {entity.Id}).",
    eventTypeCode: "MODIFICATION");

        return true;
    }
}