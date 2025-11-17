namespace FrontOffice.Domain.Enums;

/// <summary>
/// Définit les modes de règlement possibles.
/// </summary>
public enum ModeReglementEnum
{
    Virement = 1, 
    Cheque = 2,
    Especes = 3,
    MobileBanking = 4 
}