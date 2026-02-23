using FrontOffice.Application.Common.Exceptions;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Authentication.Queries.Login;

public class LoginClientQueryHandler : IRequestHandler<LoginClientQuery, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginClientQueryHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthenticationResult> Handle(LoginClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        if (client is null || string.IsNullOrEmpty(client.MotPasse) || !_passwordHasher.Verify(request.Password, client.MotPasse))
        {
            throw new UnauthorizedAccessException("L'adresse e-mail ou le mot de passe est incorrect.");
        }

        if (client.Desactive == 1)
        {
            throw new UnauthorizedAccessException("Ce compte client a été désactivé par l'administration.");
        }

        if (client.NiveauValidation == 0 || !client.IsVerified)
        {
            throw new UnauthorizedAccessException("Votre adresse e-mail n'a pas encore été vérifiée.");
        }


        var token = _jwtTokenGenerator.GenerateToken(client.Id, client.Email!);

        return new AuthenticationResult
        {
            Message = "Connexion réussie.",
            Token = token
        };
    }
}