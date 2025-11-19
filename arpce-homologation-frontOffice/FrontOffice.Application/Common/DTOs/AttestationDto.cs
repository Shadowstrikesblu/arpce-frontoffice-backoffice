using System;

namespace FrontOffice.Application.Common.DTOs;

public class AttestationDto
{
    public Guid Id { get; set; }
    public DateTime DateDelivrance { get; set; }
    public DateTime DateExpiration { get; set; }
    // On ne retourne pas les données binaires (byte[]), mais peut-être un lien de téléchargement si nécessaire.
}