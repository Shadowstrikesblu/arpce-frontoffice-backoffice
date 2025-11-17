using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Gère la logique de la commande d'inscription d'un nouveau client.
/// </summary>
public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    /// <param name="context">Le contexte de la base de données.</param>
    /// <param name="jwtTokenGenerator">Le service de génération de token JWT.</param>
    /// <param name="passwordHasher">Le service de hachage de mot de passe.</param>
    public RegisterClientCommandHandler(
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
    /// <param name="request">La commande d'inscription contenant les informations du client.</param>
    /// <param name="cancellationToken">Le token d'annulation.</param>
    /// <returns>Un résultat d'authentification contenant un message de succès et un token JWT.</returns>
    /// <exception cref="InvalidOperationException">Levée si un client avec le même email existe déjà.</exception>
    public async Task<AuthenticationResult> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        // Vérifie si un client avec le même email existe déjà
        if (await _context.Clients.AnyAsync(c => c.Email == request.Email, cancellationToken))
        {
            // Exception
            throw new InvalidOperationException("Un client avec cet email existe déjà.");
        }

        // Hashe le mot de passe en utilisant le service BCrypt
        var hashedPassword = _passwordHasher.Hash(request.Password);

        // Crée une nouvelle instance de l'entité Client
        var client = new Client
        {
            Id = Guid.NewGuid(),
            RaisonSociale = request.RaisonSociale,
            Email = request.Email,
            MotPasse = hashedPassword,
            ContactNom = request.ContactNom,
            ContactTelephone = request.ContactTelephone,
            // Génération de code temporaire
            Code = $"CLT-{DateTime.UtcNow.Ticks}", 
            DateCreation = DateTime.UtcNow,
            // Le client est actif par défaut
            Desactive = 0,
            // Changement de mot de passe à la première connexion
            ChangementMotPasse = 1 
        };

        // Ajout du nouveau client au contexte de la base de données
        _context.Clients.Add(client);

        // Sauvegarde les changements dans la base de données
        await _context.SaveChangesAsync(cancellationToken);

        // Génére un token JWT pour la session de l'utilisateur nouvellement créé
        var token = _jwtTokenGenerator.GenerateToken(client.Id, client.Email);

        // Retourne le résultat avec un message de succès et le token
        return new AuthenticationResult
        {
            Message = "Inscription réussie.",
            Token = token
        };
    }
}