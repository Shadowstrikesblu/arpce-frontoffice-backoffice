using BackOffice.Application.Common.DTOs;

namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList
{
    public class ProfilFullDto
    {
            public Guid Id { get; set; }
            public string? Code { get; set; }
            public string Libelle { get; set; } = string.Empty;
            public string? Remarques { get; set; }
            public string? UtilisateurCreation { get; set; }
            public DateTime? DateCreation { get; set; }
            public List<AdminProfilAccesDto> Acces { get; set; } = new();
            public List<AdminProfilsUtilisateursLDAPDto> UtilisateursLDAP { get; set; } = new();
            public List<AdminUserSimpleDto> Utilisateurs { get; set; } = new();
        }
    }
