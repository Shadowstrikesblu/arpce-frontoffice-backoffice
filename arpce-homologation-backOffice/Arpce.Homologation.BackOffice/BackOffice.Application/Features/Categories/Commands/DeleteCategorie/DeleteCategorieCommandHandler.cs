using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.DeleteCategorie;

public class DeleteCategorieCommandHandler : IRequestHandler<DeleteCategorieCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategorieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCategorieCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoriesEquipements
            .FindAsync(new object[] { request.CategorieId }, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"La catégorie avec l'ID '{request.CategorieId}' est introuvable.");
        }

        _context.CategoriesEquipements.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}