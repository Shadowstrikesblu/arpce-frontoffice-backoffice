using BackOffice.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Commands.ValidateRedevable;

/// <summary>
/// Gère la validation administrative d'un compte redevable (Passage du Niveau 1 au Niveau 2).
/// </summary>
public class ValidateRedevableCommandHandler : IRequestHandler<ValidateRedevableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public ValidateRedevableCommandHandler(IApplicationDbContext context, IEmailService emailService, IAuditService auditService)
    {
        _context = context;
        _emailService = emailService;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ValidateRedevableCommand request, CancellationToken cancellationToken)
    {
        // Récupére le client
        var client = await _context.Clients.FindAsync(new object[] { request.RedevableId }, cancellationToken);

        if (client == null)
        {
            throw new Exception($"Redevable avec l'ID '{request.RedevableId}' introuvable.");
        }

        // Vérifie le statut actuel
        if (client.NiveauValidation >= 2)
        {
            // Déjà validé, on retourne true sans erreur (idempotence) ou on lève une exception si on préfère.
            return true;
        }

        // Mettre à jour le niveau de validation
        client.NiveauValidation = 2; // Validé ARPCE

        // On s'assure aussi qu'il n'est pas désactivé
        client.Desactive = 0;

        await _context.SaveChangesAsync(cancellationToken);

        // Envoye l'e-mail de notification au client
        if (!string.IsNullOrEmpty(client.Email))
        {
            var subject = "Validation de votre compte ARPCE Homologation";
            var body = $@"
                <h1>Félicitations !</h1>
                <p>Votre compte pour la société <strong>{client.RaisonSociale}</strong> a été validé par l'administration de l'ARPCE.</p>
                <p>Vous pouvez dès à présent vous connecter à votre espace et déposer vos demandes d'homologation.</p>
                <p>Cordialement,<br/>L'équipe ARPCE</p>";

            try
            {
                await _emailService.SendEmailAsync(client.Email, subject, body);
            }
            catch
            {
                // On ignore l'erreur d'envoi d'email pour ne pas bloquer la transaction
            }
        }

        await _auditService.LogAsync(
            page: "Validation Redevable",
            libelle: $"Le compte du redevable '{client.RaisonSociale}' (ID: {client.Id}) a été validé (Niveau 2).",
            eventTypeCode: "VALIDATION");

        return true;
    }
}