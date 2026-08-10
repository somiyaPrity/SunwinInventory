namespace SunwinInventory.Services.Pdf
{
    public interface IInvoicePdfService
    {
        Task<byte[]> GenerateInvoiceAsync(int salesInvoiceId);
    }
}