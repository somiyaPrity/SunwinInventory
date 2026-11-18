namespace SunwinInventory.DTOs.FinancialReport
{
    public class BalanceSheetDto
    {
        public DateTime AsOfDate { get; set; }

        public DateTime OpeningDate { get; set; }

        public decimal StockValue { get; set; }

        public int StockQuantity { get; set; }

        public decimal Receivables { get; set; }

        public int DueCustomerCount { get; set; }

        public decimal Cash { get; set; }

        public decimal Bank { get; set; }

        public decimal Wallet { get; set; }

        public decimal TotalAssets =>
            StockValue + Receivables + Cash + Bank + Wallet;

        public decimal VatAitPayable { get; set; }

        public decimal TotalLiabilities => VatAitPayable;

        public decimal RetainedEarnings { get; set; }

        public decimal BalancingFigure =>
            TotalAssets - TotalLiabilities - RetainedEarnings;

        public decimal TotalEquity =>
            RetainedEarnings + BalancingFigure;

        public decimal TotalLiabilitiesAndEquity =>
            TotalLiabilities + TotalEquity;

        public string Title =>
            $"BALANCE SHEET AS ON {AsOfDate:dd-MMM-yyyy}".ToUpper();
    }
}