using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.DeleteRedevable;

public class DeleteRedevableCommandHandler : IRequestHandler<DeleteRedevableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public DeleteRedevableCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteRedevableCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(new object[] { request.Id }, cancellationToken);
        if (client == null) throw new Exception("Redevable introuvable.");

        client.Desactive = 1;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Gestion des Redevables",
            libelle: $"Le compte redevable '{client.RaisonSociale}' (ID: {client.Id}) a été désactivé (Soft Delete).",
            eventTypeCode: "SUPPRESSION");

        return true;
    }
}