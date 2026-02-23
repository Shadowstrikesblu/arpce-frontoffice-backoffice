using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace FrontOffice.Application.Features.Demandes.Commands.UploadDossierFiles;

public class UploadDossierFilesCommand : IRequest<bool>
{
    public Guid DossierId { get; set; }
    public IFormFile LettreDemande { get; set; } = default!;
    public IFormFile FicheTechnique { get; set; } = default!;
    public List<IFormFile>? DocumentsSupplementaires { get; set; }
    public List<string>? LibellesDocumentsSupplementaires { get; set; }
}