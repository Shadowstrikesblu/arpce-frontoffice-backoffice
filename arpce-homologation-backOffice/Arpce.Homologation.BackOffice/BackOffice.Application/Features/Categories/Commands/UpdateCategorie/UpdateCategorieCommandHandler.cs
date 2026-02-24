using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Categories.Commands.UpdateCategorie;

public class UpdateCategorieCommandHandler : IRequestHandler<UpdateCategorieCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly ILogger<UpdateCategorieCommandHandler> _logger;

    public UpdateCategorieCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService,
        IEmailService emailService,
        ILogger<UpdateCategorieCommandHandler> logger)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateCategorieCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoriesEquipements
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"La catégorie avec l'ID '{request.Id}' est introuvable.");
        }

        if (request.Code != null) entity.Code = request.Code;
        if (request.Libelle != null) entity.Libelle = request.Libelle;
        if (request.TypeEquipement != null) entity.TypeEquipement = request.TypeEquipement;
        if (request.TypeClient != null) entity.TypeClient = request.TypeClient;
        if (request.FraisEtude.HasValue) entity.FraisEtude = request.FraisEtude;
        if (request.FraisHomologation.HasValue) entity.FraisHomologation = request.FraisHomologation;
        if (request.FraisControle.HasValue) entity.FraisControle = request.FraisControle;
        if (request.FormuleHomologation != null) entity.FormuleHomologation = request.FormuleHomologation;
        if (request.QuantiteReference.HasValue) entity.QuantiteReference = request.QuantiteReference;
        if (request.Remarques != null) entity.Remarques = request.Remarques;

        if (request.ModeCalcul.HasValue) entity.ModeCalcul = request.ModeCalcul.Value;
        if (request.BlockSize.HasValue) entity.BlockSize = request.BlockSize;
        if (request.QtyMin.HasValue) entity.QtyMin = request.QtyMin;
        if (request.QtyMax.HasValue) entity.QtyMax = request.QtyMax;
        if (request.ReferenceLoiFinance != null) entity.ReferenceLoiFinance = request.ReferenceLoiFinance;
        if (request.CoutUnitaire.HasValue) entity.CoutUnitaire = request.CoutUnitaire.Value;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Gestion des Catégories",
            libelle: $"Modification des paramètres de la catégorie '{entity.Code}'.",
            eventTypeCode: "MODIFICATION");

        string sujet = "Mise à jour tarifaire / Catégorie";
        string message = $"La catégorie d'équipement '{entity.Libelle}' ({entity.Code}) a été modifiée par un administrateur.";

        await _notificationService.SendToGroupAsync("ADMIN", sujet, message, "Warning");

        var admins = await _context.AdminUtilisateurs
            .AsNoTracking()
            .Where(u => u.Profil.Code == "ADMIN" && !u.Desactive)
            .Select(u => u.Email ?? u.Compte) 
            .ToListAsync(cancellationToken);

        foreach (var email in admins.Where(e => e.Contains("@")))
        {
            try
            {
                string body = $@"
                    <div style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #CE2A2D;'>Alerte Modification Paramétrage</h2>
                        <p>{message}</p>
                        <ul>
                            <li><strong>Code :</strong> {entity.Code}</li>
                            <li><strong>Nouveau Mode :</strong> {entity.ModeCalcul}</li>
                            <li><strong>Réf Loi Finance :</strong> {entity.ReferenceLoiFinance ?? "N/A"}</li>
                        </ul>
                        <p>Veuillez vérifier la cohérence des nouveaux tarifs dans le module d'administration.</p>
                    </div>";

                await _emailService.SendEmailAsync(email, sujet, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du mail de modification de catégorie à {Email}", email);
            }
        }

        return true;
    }
}