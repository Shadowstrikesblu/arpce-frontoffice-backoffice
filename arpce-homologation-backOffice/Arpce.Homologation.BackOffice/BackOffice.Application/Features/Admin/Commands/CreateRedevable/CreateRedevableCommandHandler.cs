// Fichier : BackOffice.Application/Features/Admin/Commands/CreateRedevable/CreateRedevableCommandHandler.cs
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Commands.CreateRedevable;

public class CreateRedevableCommandHandler : IRequestHandler<CreateRedevableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditService _auditService;
    public CreateRedevableCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IAuditService auditService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _auditService = auditService;
    }

    public async Task<bool> Handle(CreateRedevableCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = !string.IsNullOrEmpty(request.MotPasse)
            ? _passwordHasher.Hash(request.MotPasse)
            : null;

        var client = new Client
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            RaisonSociale = request.RaisonSociale,
            RegistreCommerce = request.RegistreCommerce,
            MotPasse = hashedPassword,
            // Conversion bool? -> byte?
            Desactive = request.Desactive.HasValue ? (request.Desactive.Value ? (byte)1 : (byte)0) : null,
            ContactNom = request.ContactNom,
            ContactTelephone = request.ContactTelephone,
            ContactFonction = request.ContactFonction,
            Email = request.Email,
            Adresse = request.Adresse,
            Bp = request.Bp,
            Ville = request.Ville,
            Pays = request.Pays,
            Remarques = request.Remarques,
            IsVerified = true,
            ChangementMotPasse = 1,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion des Redevables",
    libelle: $"Création du redevable '{request.RaisonSociale}' (Email: {request.Email}).",
    eventTypeCode: "CREATION");

        return true;
    }
}