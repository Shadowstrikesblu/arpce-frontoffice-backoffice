public interface IReceiptGeneratorService
{
    Task<byte[]> GenerateReceiptPdfAsync(Guid dossierId, decimal montant, string mode, string quittance);
}