namespace BackOffice.Application.Features.Admin.Commands.CreateProfil;

public class ProfilAccessDto
{
    public Guid IdAccess { get; set; }
    public bool Ajouter { get; set; }
    public bool Valider { get; set; }
    public bool Supprimer { get; set; }
    public bool Imprimer { get; set; }
}