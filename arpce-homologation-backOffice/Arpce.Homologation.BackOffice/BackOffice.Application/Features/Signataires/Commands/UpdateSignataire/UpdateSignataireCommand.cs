using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Signataires.Commands.UpdateSignataire
{
    public class UpdateSignataireCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenoms { get; set; }
        public string? Fonction { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile? SignatureFile { get; set; }
    }
}
