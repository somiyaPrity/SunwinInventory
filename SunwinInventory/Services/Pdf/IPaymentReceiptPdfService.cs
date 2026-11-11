namespace SunwinInventory.Services.Pdf
{
    public interface IPaymentReceiptPdfService
    {
        Task<byte[]> GeneratePaymentReceiptAsync(int customerPaymentId);
    }
}