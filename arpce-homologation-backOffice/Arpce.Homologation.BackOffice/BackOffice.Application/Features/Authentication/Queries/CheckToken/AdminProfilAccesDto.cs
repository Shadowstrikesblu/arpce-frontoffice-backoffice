namespace BackOffice.Application.Features.Authentication.Queries.CheckToken;

public class AdminProfilAccesDto
{
    public Guid IdProfil { get; set; }
    public Guid IdAccess { get; set; }
    public bool Ajouter { get; set; }
    public bool Valider { get; set; }
    public bool Supprimer { get; set; }
    public bool Imprimer { get; set; }

    public AdminAccessDto Access { get; set; } = default!;
}