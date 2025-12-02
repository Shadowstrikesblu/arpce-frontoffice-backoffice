using MediatR;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Admin.Commands.UpdateRedevable;

public class UpdateRedevableCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid Id { get; set; } 

    public string? RaisonSociale { get; set; }
    public string? Email { get; set; }
    public string? MotPasse { get; set; } 
    public string? ContactNom { get; set; }
    public string? ContactTelephone { get; set; }
    public string? TypeClient { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
}