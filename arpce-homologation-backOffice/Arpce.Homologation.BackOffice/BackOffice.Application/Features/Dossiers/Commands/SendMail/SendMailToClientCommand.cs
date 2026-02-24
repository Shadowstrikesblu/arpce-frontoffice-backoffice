using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Features.Dossiers.Commands.SendMail;

public class SendMailToClientCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid DossierId { get; set; }
    public string Sujet { get; set; } = string.Empty;
    public string Corps { get; set; } = string.Empty;

    public List<IFormFile>? Attachments { get; set; }

    public string? NouveauStatusCode { get; set; }
}