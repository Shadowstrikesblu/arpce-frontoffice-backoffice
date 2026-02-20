using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Signataires.Queries.GetSignataireById;

public class GetSignataireByIdQuery : IRequest<SignataireDto?>
{
    public Guid Id { get; set; }
}

public class GetSignataireByIdQueryHandler : IRequestHandler<GetSignataireByIdQuery, SignataireDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSignataireByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SignataireDto?> Handle(GetSignataireByIdQuery request, CancellationToken cancellationToken)
    {
        var s = await _context.Signataires
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (s == null) return null;

        return new SignataireDto
        {
            Id = s.Id,
            Nom = s.Nom,
            Prenoms = s.Prenoms,
            Fonction = s.Fonction,
            SignatureImageUrl = s.SignatureImagePath,
            IsActive = s.IsActive,
            AdminId = s.AdminId
        };
    }
}