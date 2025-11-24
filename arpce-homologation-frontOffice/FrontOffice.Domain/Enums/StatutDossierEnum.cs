namespace FrontOffice.Domain.Enums;

public enum StatutDossierEnum
{
    NouveauDossier = 1,         // 01
    Instruction = 2,            // 02
    ApprobationInstruction = 3, // 03
    InstructionApprouve = 4,    // New
    DevisEmis = 5,              // 03B
    DevisValide = 6,            // 03C
    DevisRejete = 7,            // New
    DevisPaiement = 8,          // 04
    PaiementRejete = 9,         // 04A
    PaiementExpire = 10,        // 04B 
    DossierPaye = 11,           // 07
    DossierSignature = 12,      // New 
    DossierSigne = 13
}