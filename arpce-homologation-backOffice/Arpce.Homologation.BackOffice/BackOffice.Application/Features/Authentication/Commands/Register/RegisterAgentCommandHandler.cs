using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Gère la logique de la commande d'inscription d'un nouvel agent.
/// </summary>
public class RegisterAgentCommandHandler : IRequestHandler<RegisterAgentCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public RegisterAgentCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Exécute la logique de la commande d'inscription.
    /// </summary>
    public async Task<AuthenticationResult> Handle(RegisterAgentCommand request, CancellationToken cancellationToken)
    {
        // Vérifie si un agent avec le même nom de compte existe déjà.
        if (await _context.AdminUtilisateurs.AnyAsync(u => u.Compte == request.Compte, cancellationToken))
        {
            throw new InvalidOperationException("Un agent avec ce nom de compte existe déjà.");
        }

        // Hache le mot de passe fourni.
        var hashedPassword = _passwordHasher.Hash(request.Password);

        // Crée la nouvelle entité AdminUtilisateur.
        var agent = new AdminUtilisateur
        {
            Id = Guid.NewGuid(),
            Compte = request.Compte,
            Nom = request.Nom,
            Prenoms = request.Prenoms,
            MotPasse = hashedPassword,
            Desactive = false, 
            ChangementMotPasse = true 
        };

        // Ajoute le nouvel agent au contexte et sauvegarde dans la base de données.
        _context.AdminUtilisateurs.Add(agent);
        await _context.SaveChangesAsync(cancellationToken);

        // Génére un token JWT pour la session de l'agent nouvellement créé.
        var token = _jwtTokenGenerator.GenerateToken(agent.Id, agent.Compte);

        // Retourne le résultat avec un message de succès et le token.
        return new AuthenticationResult
        {
            Message = "Inscription de l'agent réussie.",
            Token = token
        };
    }
}