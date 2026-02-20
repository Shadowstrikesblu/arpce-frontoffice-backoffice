using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Dossiers.Commands.ValidateInstruction;

public class ValidateInstructionCommandHandler : IRequestHandler<ValidateInstructionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ValidateInstructionCommandHandler> _logger;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public ValidateInstructionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ValidateInstructionCommandHandler> logger,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(ValidateInstructionCommand request, CancellationToken cancellationToken)
    {
        var agentId = _currentUserService.UserId;
        if (!agentId.HasValue)
        {
            throw new UnauthorizedAccessException("Accès non autorisé. L'authentification de l'agent est requise.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dossier = await _context.Dossiers
                .Include(d => d.Demande) // Correction : Propriété unique
                    .ThenInclude(dem => dem.CategorieEquipement)
                .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

            if (dossier == null)
            {
                throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");
            }

            var existingDevis = await _context.Devis.FirstOrDefaultAsync(d => d.IdDossier == dossier.Id && d.PaiementOk != 1, cancellationToken);
            if (existingDevis == null && dossier.Demande != null)
            {
                decimal totalFraisEtude = dossier.Demande.CategorieEquipement?.FraisEtude ?? 0;
                decimal totalFraisHomologation = dossier.Demande.CategorieEquipement?.FraisHomologation ?? 0;
                decimal totalFraisControle = dossier.Demande.CategorieEquipement?.FraisControle ?? 0;

                var nouveauDevis = new Devis
                {
                    Id = Guid.NewGuid(),
                    IdDossier = dossier.Id,
                    Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    MontantEtude = totalFraisEtude,
                    MontantHomologation = totalFraisHomologation,
                    MontantControle = totalFraisControle,
                    PaiementOk = 0
                };
                _context.Devis.Add(nouveauDevis);
            }

            var statutCibleCode = StatutDossierEnum.InstructionApprouve.ToString();
            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == statutCibleCode, cancellationToken);
            if (nouveauStatut != null)
            {
                dossier.IdStatut = nouveauStatut.Id;
            }

            if (!string.IsNullOrWhiteSpace(request.Remarque))
            {
                var agent = await _context.AdminUtilisateurs.FindAsync(agentId.Value);
                var nomAgent = agent != null ? $"{agent.Prenoms} {agent.Nom}" : "Agent Inconnu";
                _context.Commentaires.Add(new Commentaire { Id = Guid.NewGuid(), IdDossier = dossier.Id, DateCommentaire = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), CommentaireTexte = request.Remarque, NomInstructeur = nomAgent });
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Validation Dossier", $"Dossier '{dossier.Numero}' validé.", "VALIDATION", dossier.Id);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}