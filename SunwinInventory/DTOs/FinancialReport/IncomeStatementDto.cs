namespace SunwinInventory.DTOs.FinancialReport
{
    public class IncomeStatementDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int InvoiceCount { get; set; }

        public int SoldQuantity { get; set; }

        public int CostlessLineCount { get; set; }

        public decimal Sales { get; set; }

        public decimal Discount { get; set; }

        public decimal NetSales => Sales - Discount;

        public decimal Cogs { get; set; }

        public decimal GrossProfit => NetSales - Cogs;

        public decimal CuttingDelivery { get; set; }

        public decimal OtherIncome { get; set; }

        public decimal OperatingExpenses { get; set; }

        public decimal OtherExpenses { get; set; }

        public decimal NetProfit =>
            GrossProfit + CuttingDelivery + OtherIncome
            - OperatingExpenses - OtherExpenses;

        public decimal Vat { get; set; }

        public decimal Ait { get; set; }

        public List<IncomeStatementFundRowDto> FundRows { get; set; } = new();

        public List<IncomeStatementCogsRowDto> CogsByProduct { get; set; } = new();

        public string Title =>
            $"INCOME STATEMENT {FromDate:dd-MMM-yyyy} TO {ToDate:dd-MMM-yyyy}".ToUpper();
    }

    public class IncomeStatementFundRowDto
    {
        public DateTime EntryDate { get; set; }

        public string Head { get; set; } = null!;

        public string? Details { get; set; }

        public string Account { get; set; } = null!;

        public decimal Amount { get; set; }
    }

    public class IncomeStatementCogsRowDto
    {
        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal WeightKg { get; set; }

        public decimal SalesAmount { get; set; }

        public decimal Cogs { get; set; }
    }
}