using FrontOffice.Application.Common.DTOs;
using MediatR;
using System;

namespace FrontOffice.Application.Features.Demandes.Commands.CreateDossier;

public class CreateDossierCommand : IRequest<Guid>
{
    public string Libelle { get; set; } = string.Empty;
    public bool EstHomologable { get; set; } = true;

    public DemandeCommandDto Demande { get; set; } = new();
}