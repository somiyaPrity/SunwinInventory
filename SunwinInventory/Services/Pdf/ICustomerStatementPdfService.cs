namespace SunwinInventory.Services.Pdf
{
    public interface ICustomerStatementPdfService
    {
        Task<byte[]> GenerateStatementAsync(int customerId);
    }
}