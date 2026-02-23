using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        // Vérification de l'unicité du compte
        if (await _context.AdminUtilisateurs.AnyAsync(u => u.Compte == request.Compte, cancellationToken))
        {
            throw new InvalidOperationException("Un agent avec ce nom de compte existe déjà.");
        }

        // Vérification de l'unicité de l'email (si fourni)
        if (!string.IsNullOrEmpty(request.Email) &&
            await _context.AdminUtilisateurs.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            throw new InvalidOperationException("Un agent avec cette adresse e-mail existe déjà.");
        }

        // Hachage du mot de passe
        var hashedPassword = _passwordHasher.Hash(request.Password);

        // Définition du type d'utilisateur par défaut (ID du seed pour Agent standard)
        var defaultUserTypeId = new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6");

        // Création de l'entité AdminUtilisateur
        var agent = new AdminUtilisateur
        {
            Id = Guid.NewGuid(),
            Compte = request.Compte,
            Email = request.Email,
            Nom = request.Nom,
            Prenoms = request.Prenoms,
            MotPasse = hashedPassword,
            IdUtilisateurType = defaultUserTypeId, 
            Desactive = false,
            ChangementMotPasse = true,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_REGISTER"
        };

        // Sauvegarde en base de données
        _context.AdminUtilisateurs.Add(agent);
        await _context.SaveChangesAsync(cancellationToken);

        // Génération du token (On utilise l'email s'il existe, sinon le compte)
        var token = _jwtTokenGenerator.GenerateToken(agent.Id, agent.Email ?? agent.Compte);

        // Retou du résultat
        return new AuthenticationResult
        {
            Message = "Inscription de l'agent réussie.",
            Token = token
        };
    }
}