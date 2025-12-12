namespace FrontOffice.Domain.Entities;

public class AdminOptions
{
    /// <summary>
    /// Clé primaire synthétique pour cette entité, générée par EF Core.
    /// Elle n'existait pas dans le schéma SQL original.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public byte? JournalActivation { get; set; }
    public byte? EventConnexion { get; set; }
    public byte? EventOuverturePage { get; set; }
    public byte? EventChangementMotPasse { get; set; }
    public byte? EventModificationDonnees { get; set; }
    public byte? EventSuppressionDonnees { get; set; }
    public byte? EventImpression { get; set; }
    public byte? EventValidation { get; set; }
    public byte? JournalLimitation { get; set; }
    public byte? JournalTypeLimitation { get; set; }
    public int? JournalDureeLimitation { get; set; }
    public byte? JournalTypeDureeLimitation { get; set; }
    public int? JournalTailleLimitation { get; set; }
    public byte? LDAPAuthentificationActivation { get; set; }
    public string? LDAPAuthentificationNomDomaine { get; set; }
    public byte? LDAPCreationAutoActivation { get; set; }
    public string? LDAPCreationAutoNomServeur { get; set; }
    public string? LDAPCreationAutoCompte { get; set; }
    public string? LDAPCreationAutoPassword { get; set; }
    public byte? ReplicationActivation { get; set; }
    public string? ReplicationNomServeur { get; set; }
    public string? ReplicationCompte { get; set; }
    public string? ReplicationPassword { get; set; }
}