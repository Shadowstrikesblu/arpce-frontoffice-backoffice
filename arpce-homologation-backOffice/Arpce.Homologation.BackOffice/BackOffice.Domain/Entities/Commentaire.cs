using BackOffice.Domain.Common; 
namespace BackOffice.Domain.Entities;
public class Commentaire : AuditableEntity 
{
    public Guid IdDossier { get; set; }
    public DateTime DateCommentaire { get; set; }
    public string? CommentaireTexte { get; set; }
    public string? NomInstructeur { get; set; }
    public string? Proposition { get; set; } 
    public virtual Dossier Dossier { get; set; } = default!;
}