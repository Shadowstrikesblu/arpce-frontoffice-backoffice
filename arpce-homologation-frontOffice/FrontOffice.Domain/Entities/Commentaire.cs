using FrontOffice.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontOffice.Domain.Entities;

public class Commentaire : AuditableEntity
{
    public Guid IdDossier { get; set; }
    public long DateCommentaire { get; set; }
    public string? CommentaireTexte { get; set; }
    public string? NomInstructeur { get; set; }
    public string? Proposition { get; set; }

    [ForeignKey(nameof(IdDossier))]
    public virtual Dossier Dossier { get; set; } = default!;
}