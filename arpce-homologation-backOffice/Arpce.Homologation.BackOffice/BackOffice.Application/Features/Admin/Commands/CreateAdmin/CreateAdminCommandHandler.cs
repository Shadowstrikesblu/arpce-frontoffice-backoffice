using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.CreateAdmin;

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditService _auditService;

    public CreateAdminCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IAuditService auditService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _auditService = auditService;
    }

    public async Task<bool> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = !string.IsNullOrEmpty(request.MotPasse)
            ? _passwordHasher.Hash(request.MotPasse)
            : null;

        var admin = new AdminUtilisateur
        {
            Id = Guid.NewGuid(),
            Compte = request.Compte,
            Nom = request.Nom,
            Prenoms = request.Prenoms,
            MotPasse = hashedPassword,
            Desactive = request.Desactive,
            Remarques = request.Remarques,

            IdUtilisateurType = request.IdUtilisateurType,

            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };

        _context.AdminUtilisateurs.Add(admin);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion des Utilisateurs",
    libelle: $"Création de l'utilisateur admin '{request.Compte}'.",
    eventTypeCode: "CREATION");

        return true;
    }
}