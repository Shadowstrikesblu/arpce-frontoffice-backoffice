using MediatR;
using System;

namespace BackOffice.Application.Features.Admin.Commands.CreateAdmin;

public class CreateAdminCommand : IRequest<bool>
{
    public string Compte { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Prenoms { get; set; }
    public string? MotPasse { get; set; }
    public bool Desactive { get; set; }
    public string? Remarques { get; set; }
    public Guid? IdProfil { get; set; }
    public Guid IdUtilisateurType { get; set; }
}