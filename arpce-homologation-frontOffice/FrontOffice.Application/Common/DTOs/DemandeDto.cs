namespace FrontOffice.Application.Common.DTOs;

public class DemandeDto
{
    public Guid Id { get; set; }
    public Guid IdDossier { get; set; }
    public string? NumeroDemande { get; set; }
    public string? Equipement { get; set; }
    public string? Modele { get; set; }
    public string? Marque { get; set; }
    // Ajoutez ici les autres champs SIMPLES de Demande que vous voulez exposer
}