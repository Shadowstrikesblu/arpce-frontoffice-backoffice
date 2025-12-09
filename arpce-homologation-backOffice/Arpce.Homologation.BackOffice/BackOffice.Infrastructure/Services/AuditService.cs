using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IApplicationDbContext context, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string page, string libelle, string eventTypeCode = "MODIFICATION", Guid? dossierId = null)
    {
        var eventType = await _context.AdminEvenementsTypes.FirstOrDefaultAsync(et => et.Code == eventTypeCode);
        if (eventType == null) return; 

        var userId = _currentUserService.UserId;
        string userName = "Anonyme";
        if (userId.HasValue)
        {
            var user = await _context.AdminUtilisateurs.FindAsync(userId.Value);
            if (user != null) userName = user.Compte;
        }

        var journalEntry = new AdminJournal
        {
            Id = Guid.NewGuid(),
            Application = "BackOffice",
            AdresseIP = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "N/A",
            Utilisateur = userName,
            DateEvenement = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Page = page,
            Libelle = libelle,
            IdEvenementType = eventType.Id,
            IdDossier = dossierId 
        };

        _context.AdminJournals.Add(journalEntry);
        await _context.SaveChangesAsync(new CancellationToken());
    }
}