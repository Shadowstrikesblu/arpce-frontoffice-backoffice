using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.DTOs
{
    public class CommentaireDto
    {
        public Guid Id { get; set; }
        public DateTime DateCommentaire { get; set; }
        public string? CommentaireTexte { get; set; }
        public string? NomInstructeur { get; set; }
    }
}
