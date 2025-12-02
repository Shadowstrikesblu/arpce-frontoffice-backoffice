using MediatR;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Admin.Commands.UpdateAdmin;

public class UpdateAdminCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    public string? Compte { get; set; }
    public string? Nom { get; set; }
    public string? Prenoms { get; set; }
    public string? Remarques { get; set; }
    public Guid? IdUtilisateurType { get; set; }
}