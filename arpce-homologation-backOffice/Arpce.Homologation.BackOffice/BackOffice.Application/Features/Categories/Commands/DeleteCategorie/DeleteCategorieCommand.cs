using MediatR;

namespace BackOffice.Application.Features.Categories.Commands.DeleteCategorie;

public class DeleteCategorieCommand : IRequest<bool>
{
    public Guid CategorieId { get; set; }

    public DeleteCategorieCommand(Guid categorieId)
    {
        CategorieId = categorieId;
    }
}