using MediatR;

namespace FrontOffice.Application.Features.Documents.Queries.DownloadCertificat
{
    /// <summary>
    /// Requête pour télécharger une attestation (certificat) spécifique.
    /// </summary>
    public class DownloadCertificatQuery : IRequest<DownloadCertificatResult>
    {
        /// <summary>
        /// L'ID de l'attestation à télécharger.
        /// </summary>
        public Guid AttestationId { get; set; }

        public DownloadCertificatQuery(Guid attestationId)
        {
            AttestationId = attestationId;
        }
    }
}
