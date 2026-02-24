using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Dossiers.Commands.SendMail;

public class SendMailToClientCommandHandler : IRequestHandler<SendMailToClientCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SendMailToClientCommandHandler> _logger;

    public SendMailToClientCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IAuditService auditService,
        INotificationService notificationService,
        ILogger<SendMailToClientCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _auditService = auditService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(SendMailToClientCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null) throw new Exception("Dossier introuvable.");
        if (string.IsNullOrEmpty(dossier.Client?.Email)) throw new Exception("Le client n'a pas d'adresse e-mail.");

        await _emailService.SendEmailAsync(dossier.Client.Email, request.Sujet, request.Corps, request.Attachments);

        if (!string.IsNullOrEmpty(request.NouveauStatusCode))
        {
            var statut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == request.NouveauStatusCode, cancellationToken);
            if (statut != null)
            {
                dossier.IdStatut = statut.Id;
                dossier.DateModification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }

        _context.Commentaires.Add(new Commentaire
        {
            Id = Guid.NewGuid(),
            IdDossier = dossier.Id,
            CommentaireTexte = $"[EMAIL ENVOYÉ] Sujet: {request.Sujet}",
            DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            NomInstructeur = "Système / Agent"
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("Communication", $"Email envoyé au client: {request.Sujet}", "EMAIL", dossier.Id);

        return true;
    }
}