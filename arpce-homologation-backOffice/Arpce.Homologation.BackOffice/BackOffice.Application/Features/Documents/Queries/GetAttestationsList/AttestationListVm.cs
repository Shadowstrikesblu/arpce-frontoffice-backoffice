namespace BackOffice.Application.Features.Documents.Queries.GetAttestationsList
{
    public class AttestationListVm
    {
        public int Page { get; set; }
        public int PageTaille { get; set; }
        public List<AttestationItemDto> Attestation { get; set; } = new List<AttestationItemDto>();
    }
}
