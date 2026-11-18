using System;
using System.Collections.Generic;

namespace SunwinInventory.DTOs.Fund
{
    public class FundBookRowDto
    {
        public DateTime Date { get; set; }

        public string Details { get; set; } = "";

        public int? FundEntryId { get; set; }

        public decimal CashIncome { get; set; }

        public decimal BankIncome { get; set; }

        public decimal WalletIncome { get; set; }

        public decimal ExchangeAmount { get; set; }

        public decimal BankDeposit { get; set; }

        public decimal PurchaseCost { get; set; }

        public decimal Expense { get; set; }

        public decimal CashBalance { get; set; }

        public decimal BankBalance { get; set; }

        public decimal WalletBalance { get; set; }
    }

    public class FundBookDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime OpeningDate { get; set; }

        public decimal OpeningCash { get; set; }

        public decimal OpeningBank { get; set; }

        public decimal OpeningWallet { get; set; }

        public List<FundBookRowDto> Rows { get; set; } = new();

        public decimal TotalCashIncome { get; set; }

        public decimal TotalBankIncome { get; set; }

        public decimal TotalWalletIncome { get; set; }

        public decimal TotalExchange { get; set; }

        public decimal TotalBankDeposit { get; set; }

        public decimal TotalPurchaseCost { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal ClosingCash { get; set; }

        public decimal ClosingBank { get; set; }

        public decimal ClosingWallet { get; set; }
    }

    public class FundOpeningBalanceDto
    {
        public int FundOpeningBalanceId { get; set; }

        public string Account { get; set; } = "";

        public DateTime OpeningDate { get; set; }

        public decimal Amount { get; set; }
    }
}