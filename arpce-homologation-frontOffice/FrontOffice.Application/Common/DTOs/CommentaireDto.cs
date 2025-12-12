using System;

namespace FrontOffice.Application.Common.DTOs;

public class CommentaireDto
{
    public Guid Id { get; set; }
    public long DateCommentaire { get; set; }
    public string? CommentaireTexte { get; set; }
    public string? NomInstructeur { get; set; }
}