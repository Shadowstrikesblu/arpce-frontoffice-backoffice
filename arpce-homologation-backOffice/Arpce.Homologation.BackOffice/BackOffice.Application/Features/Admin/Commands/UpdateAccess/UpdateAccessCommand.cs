using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Commands.UpdateAccess
{
    /// <summary>
    /// Commande pour modifier la définition d'un accès (Droit) existant.
    /// </summary>
    public class UpdateAccessCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public string Groupe { get; set; } = string.Empty;
        public string Application { get; set; } = string.Empty;
        public string? Page { get; set; }
        public string Type { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool Inactif { get; set; }

        // Définition des permissions possibles sur cet accès
        public bool Ajouter { get; set; }
        public bool Valider { get; set; }
        public bool Supprimer { get; set; }
        public bool Imprimer { get; set; }
    }
}
