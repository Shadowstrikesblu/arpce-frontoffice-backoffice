using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

public class Notification : AuditableEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info";

    public string? TargetUrl { get; set; }
    public string? EntityId { get; set; }

    public Guid? UserId { get; set; }
    public string? ProfilCode { get; set; }
    public bool IsBroadcast { get; set; } = false;
    public bool IsRead { get; set; } = false;
    public long DateEnvoi { get; set; }

    public string Canal { get; set; } = "SYSTEM"; 
    public string? Destinataire { get; set; }    
    public string? StatutEnvoi { get; set; }     
}