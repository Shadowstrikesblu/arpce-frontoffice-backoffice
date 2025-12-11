namespace FrontOffice.Domain.Enums;

/// <summary>
/// Statuts du dossier d'homologation 
/// </summary>

public enum StatutDossierEnum
{
    NouveauDossier = 1,
    RefusDossier = 2,
    Instruction = 3,
    ApprobationInstruction = 4,
    InstructionApprouve = 5,
    DevisCreer = 6,
    DevisValideSC = 7,
    DevisValideTr = 8,
    DevisEmit = 9,
    DevisValide = 10,
    DevisRefuser = 11,
    DevisPaiement = 12, 
    PaiementRejete = 13,
    PaiementExpirer = 14,
    DossierPayer = 15, 
    DossierSignature = 16,
    DossierSigne = 17,
    Echantillon = 18,
    Exempt = 99,
    Paid = 100 
}