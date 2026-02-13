namespace BackOffice.Domain.Enums;

public enum StatutDemandeEnum
{
    /// <summary>
    /// L'équipement est en attente d'un échantillon physique pour test.
    /// </summary>
    Echantillon = 1,

    /// <summary>
    /// L'équipement a été refusé après instruction.
    /// </summary>
    Refus = 2,

    /// <summary>
    /// L'attestation de l'équipement a été signée.
    /// </summary>
    Signe = 3,

    Accepte = 4
}