namespace BackOffice.Domain.Enums
{
    /// <summary>
    /// Définit les statuts possibles pour une demande de dossier d'homologation,
    /// basé sur le diagramme d'évolution des statuts.
    /// </summary>
    public enum StatutDossierEnum
    {
        /// <summary>
        /// statut 01 : Le dossier vient d'être créé par le client.
        /// </summary>
        NouvelleDemande,

        /// <summary>
        /// statut 02 : Un agent de l'ARPCE a commencé l'instruction de la demande.
        /// </summary>
        EnCoursInstruction,

        /// <summary>
        /// statut 03 : La demande a été envoyée pour approbation à un responsable.
        /// </summary>
        EnvoyePourApprobation,

        /// <summary>
        /// statut 04 : La demande a été approuvée et est en attente de paiement par le client.
        /// </summary>
        ApprouveAttentePaiement,

        /// <summary>
        /// statut 05 : La demande a été rejetée par un responsable.
        /// </summary>
        Rejetee,

        /// <summary>
        /// statut 06 : La demande a été approuvée mais l'équipement n'est pas soumis à homologation.
        /// </summary>
        EquipementNonSoumisAHomologation,

        /// <summary>
        /// statut 07 : Le paiement a été effectué et vérifié.
        /// </summary>
        ApprouvePaiementEffectue,

        /// <summary>
        /// statut 08 : L'attestation a été générée, signée et chargée dans l'application.
        /// </summary>
        ApprouveAttestationSignee,

        /// <summary>
        /// Un statut pour l'annulation de l'instruction, si nécessaire.
        /// </summary>
        AnnulationInstruction
    }
}
