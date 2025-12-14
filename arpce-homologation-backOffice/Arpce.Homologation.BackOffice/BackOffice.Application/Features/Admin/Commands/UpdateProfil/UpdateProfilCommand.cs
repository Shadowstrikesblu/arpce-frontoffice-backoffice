using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Commands.UpdateProfil
{
    /// <summary>
    /// Commande pour modifier les informations d'un profil existant.
    /// </summary>
    public class UpdateProfilCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Libelle { get; set; } = string.Empty;
        public string? Remarques { get; set; }
    }
}
