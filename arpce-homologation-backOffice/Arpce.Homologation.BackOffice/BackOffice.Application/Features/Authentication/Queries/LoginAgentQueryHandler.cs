using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Authentication.Queries.Login;

public class LoginAgentQueryHandler : IRequestHandler<LoginAgentQuery, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginAgentQueryHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthenticationResult> Handle(LoginAgentQuery request, CancellationToken cancellationToken)
    {
        var agent = await _context.AdminUtilisateurs
            .Include(u => u.Profil) 
            .FirstOrDefaultAsync(u => u.Compte == request.Compte, cancellationToken);

        if (agent is null || string.IsNullOrEmpty(agent.MotPasse) || !_passwordHasher.Verify(request.Password, agent.MotPasse))
        {
            throw new UnauthorizedAccessException("Nom de compte ou mot de passe invalide.");
        }

        if (agent.Desactive)
        {
            throw new UnauthorizedAccessException("Ce compte agent est désactivé.");
        }

        agent.DerniereConnexion = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await _context.SaveChangesAsync(cancellationToken);

        // On utilise le code du profil comme nom de groupe pour SignalR
        string? profilCode = agent.Profil?.Code;
        string? groupName = agent.Profil?.Code; 

        var token = _jwtTokenGenerator.GenerateToken(agent.Id, agent.Compte, profilCode, groupName);

        return new AuthenticationResult
        {
            Message = "Connexion réussie.",
            Token = token
        };
    }
}