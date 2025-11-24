namespace BackOffice.Domain.Entities;

public class AdminProfilsUtilisateursLDAP
{
    public string Utilisateur { get; set; } = string.Empty;
    public Guid IdProfil { get; set; }

    public virtual AdminProfils? Profil { get; set; }
}