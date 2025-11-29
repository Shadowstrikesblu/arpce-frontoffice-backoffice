using MediatR;
using System.Text.Json.Serialization;

public class ChangeDossierStatusCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DossierId { get; set; }
    public string CodeStatut { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
}