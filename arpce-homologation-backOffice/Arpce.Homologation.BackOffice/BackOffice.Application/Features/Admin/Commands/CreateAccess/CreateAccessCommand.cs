using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.CreateAccess;

public class CreateAccessCommand : IRequest<bool>
{
    public string Application { get; set; } = string.Empty;
    public string Groupe { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string Page { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Inactif { get; set; }
    public bool Ajouter { get; set; }
    public bool Valider { get; set; }
    public bool Supprimer { get; set; }
    public bool Imprimer { get; set; }
}