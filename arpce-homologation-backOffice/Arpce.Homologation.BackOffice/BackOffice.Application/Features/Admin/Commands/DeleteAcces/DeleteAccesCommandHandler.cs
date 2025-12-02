using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.DeleteAcces;

public class DeleteAccesCommandHandler : IRequestHandler<DeleteAccesCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    public DeleteAccesCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteAccesCommand request, CancellationToken cancellationToken)
    {
        var acces = await _context.AdminAccesses.FindAsync(new object[] { request.Id }, cancellationToken);

        if (acces == null)
        {
            throw new Exception($"L'accès avec l'ID '{request.Id}' est introuvable.");
        }

        _context.AdminAccesses.Remove(acces);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion des Profils",
    libelle: $"Suppression du profil '{request.Code}' (ID: {request.Id}).",
    eventTypeCode: "SUPPRESSION");

        return true;
    }
}