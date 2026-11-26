using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.Fund;
using SunwinInventory.Models;

namespace SunwinInventory.Controllers
{
    public class FundController : Controller
    {
        private static readonly string[] Heads =
        {
            "Other Income",
            "Bank Deposit",
            "TAX/CNF",
            "Foreign Purchase",
            "Local Purchase",
            "Operating Expense",
            "Other Expense"
        };

        private static readonly string[] Accounts =
        {
            "Cash", "Bank", "Wallet"
        };

        private readonly SunwinInventoryContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FundController(
            SunwinInventoryContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CurrentUserName =>
            _httpContextAccessor.HttpContext?
                .Session.GetString("UserName") ?? "System";

        // =========================================================
        // FUND BOOK REPORT
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? fromDate,
            DateTime? toDate)
        {
            var model = await BuildBookAsync(fromDate, toDate);

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;

            return View(model);
        }

        // =========================================================
        // ENTRY CREATE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadEntryLookups();

            return View(new FundEntryDto
            {
                EntryDate = ViewerToday(),
                Account = "Cash"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FundEntryDto model)
        {
            NormalizeEntry(model);

            if (!ModelState.IsValid)
            {
                await LoadEntryLookups();
                return View(model);
            }

            var openingDate = await GetOpeningDateAsync();

            if (model.EntryDate!.Value.Date < openingDate)
            {
                ModelState.AddModelError(
                    nameof(model.EntryDate),
                    $"Entry date cannot be before the opening date {openingDate:dd-MMM-yyyy}.");

                await LoadEntryLookups();
                return View(model);
            }

            try
            {
                _context.FundEntries.Add(new FundEntry
                {
                    EntryDate = model.EntryDate.Value.Date,
                    Head = model.Head,
                    Account = model.Account,
                    Details = model.Details,
                    Amount = model.Amount,
                    CreatedBy = CurrentUserName,
                    CreatedDate = DateTime.Now
                });

                await _context.SaveChangesAsync();

                TempData["Success"] = "Fund entry saved successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message ?? ex.Message;

                await LoadEntryLookups();
                return View(model);
            }
        }

        // =========================================================
        // ENTRY EDIT
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entry = await _context.FundEntries
                .FirstOrDefaultAsync(x => x.FundEntryId == id);

            if (entry == null)
                return NotFound();

            await LoadEntryLookups();

            return View(new FundEntryDto
            {
                FundEntryId = entry.FundEntryId,
                EntryDate = entry.EntryDate,
                Head = entry.Head,
                Account = entry.Account,
                Details = entry.Details,
                Amount = entry.Amount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FundEntryDto model)
        {
            NormalizeEntry(model);

            if (!ModelState.IsValid)
            {
                await LoadEntryLookups();
                return View(model);
            }

            var entry = await _context.FundEntries
                .FirstOrDefaultAsync(x =>
                    x.FundEntryId == model.FundEntryId);

            if (entry == null)
                return NotFound();

            var openingDate = await GetOpeningDateAsync();

            if (model.EntryDate!.Value.Date < openingDate)
            {
                ModelState.AddModelError(
                    nameof(model.EntryDate),
                    $"Entry date cannot be before the opening date {openingDate:dd-MMM-yyyy}.");

                await LoadEntryLookups();
                return View(model);
            }

            try
            {
                entry.EntryDate = model.EntryDate.Value.Date;
                entry.Head = model.Head;
                entry.Account = model.Account;
                entry.Details = model.Details;
                entry.Amount = model.Amount;
                entry.UpdatedBy = CurrentUserName;
                entry.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Fund entry updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message ?? ex.Message;

                await LoadEntryLookups();
                return View(model);
            }
        }

        // =========================================================
        // ENTRY DELETE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entry = await _context.FundEntries
                    .FirstOrDefaultAsync(x => x.FundEntryId == id);

                if (entry != null)
                {
                    _context.FundEntries.Remove(entry);
                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        "Fund entry deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // OPENING BALANCES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> OpeningBalances()
        {
            var model = await _context.FundOpeningBalances
                .OrderBy(x => x.FundOpeningBalanceId)
                .Select(x => new FundOpeningBalanceDto
                {
                    FundOpeningBalanceId = x.FundOpeningBalanceId,
                    Account = x.Account,
                    OpeningDate = x.OpeningDate,
                    Amount = x.Amount
                })
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveOpeningBalances(
            List<FundOpeningBalanceDto> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var row = await _context.FundOpeningBalances
                        .FirstOrDefaultAsync(x =>
                            x.FundOpeningBalanceId ==
                            item.FundOpeningBalanceId);

                    if (row != null)
                        row.Amount = item.Amount;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Opening balances updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // EXCEL EXPORT
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportExcel(
            DateTime? fromDate,
            DateTime? toDate)
        {
            var model = await BuildBookAsync(fromDate, toDate);

            var bytes = BuildWorkbook(model);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"FundBook_{model.FromDate:yyyyMMdd}_{model.ToDate:yyyyMMdd}.xlsx");
        }

        // =========================================================
        // BOOK BUILDER
        // =========================================================

        private async Task<FundBookDto> BuildBookAsync(
            DateTime? fromDate,
            DateTime? toDate)
        {
            var openings = await _context.FundOpeningBalances
                .ToListAsync();

            decimal Opening(string account) =>
                openings
                    .Where(x => x.Account == account)
                    .Sum(x => x.Amount);

            var openingDate = openings.Count > 0
                ? openings.Min(x => x.OpeningDate)
                : new DateTime(2026, 6, 1);

            var today = ViewerToday();

            var from = fromDate?.Date ?? openingDate;

            if (from < openingDate)
                from = openingDate;

            var to = toDate?.Date ?? today;

            if (to < from)
                to = from;

            var end = to.AddDays(1);

            var sales = await (
                from h in _context.SalesHeaders

                join c in _context.Customers
                    on h.CustomerId equals c.CustomerId

                where h.SalesDate >= openingDate &&
                      h.SalesDate < end &&
                      (h.ReceiveAmount ?? 0) > 0

                select new
                {
                    h.SalesId,
                    Date = h.SalesDate,
                    h.InvoiceNo,
                    CustomerName = c.CustomerName,
                    Method = h.ReceiveMethod,
                    Amount = h.ReceiveAmount ?? 0
                })
                .ToListAsync();

            var dues = await (
                from p in _context.CustomerPayments

                join i in _context.SalesInvoices
                    on p.SalesInvoiceId equals i.SalesInvoiceId

                join c in _context.Customers
                    on p.CustomerId equals c.CustomerId

                where p.PaymentDate >= openingDate &&
                      p.PaymentDate < end

                select new
                {
                    p.CustomerPaymentId,
                    Date = p.PaymentDate,
                    i.InvoiceNo,
                    CustomerName = c.CustomerName,
                    Method = p.PaymentMethod,
                    Amount = p.PaymentAmount
                })
                .ToListAsync();

            var manual = await _context.FundEntries
                .Where(x => x.EntryDate >= openingDate &&
                            x.EntryDate < end)
                .OrderBy(x => x.FundEntryId)
                .ToListAsync();

            var events = new List<(DateTime Date, int Order, int Id, FundBookRowDto Row, string Account, int Direction)>();

            foreach (var s in sales)
            {
                var row = new FundBookRowDto
                {
                    Date = s.Date.Date,
                    Details =
                        $"Daily Sales Income — {s.InvoiceNo} ({s.CustomerName})"
                };

                var account = AccountForMethod(s.Method);

                SetIncome(row, account, s.Amount);

                events.Add((s.Date.Date, 0, s.SalesId, row, account, +1));
            }

            foreach (var p in dues)
            {
                var row = new FundBookRowDto
                {
                    Date = p.Date.Date,
                    Details =
                        $"Dues Collection — {p.InvoiceNo} ({p.CustomerName})"
                };

                var account = AccountForMethod(p.Method);

                SetIncome(row, account, p.Amount);

                events.Add((p.Date.Date, 1, p.CustomerPaymentId, row, account, +1));
            }

            foreach (var e in manual)
            {
                var row = new FundBookRowDto
                {
                    Date = e.EntryDate,
                    FundEntryId = e.FundEntryId,
                    Details = string.IsNullOrWhiteSpace(e.Details)
                        ? e.Head
                        : $"{e.Head}: {e.Details}"
                };

                var account = e.Account;
                int direction;

                switch (e.Head)
                {
                    case "Other Income":
                        SetIncome(row, account, e.Amount);
                        direction = +1;
                        break;

                    case "Bank Deposit":
                        row.BankDeposit = e.Amount;
                        account = "Cash";
                        direction = -1;
                        break;

                    case "TAX/CNF":
                    case "Foreign Purchase":
                    case "Local Purchase":
                        row.PurchaseCost = e.Amount;
                        row.Details += $" ({e.Account})";
                        direction = -1;
                        break;

                    default:
                        row.Expense = e.Amount;
                        row.Details += $" ({e.Account})";
                        direction = -1;
                        break;
                }

                events.Add((e.EntryDate, 2, e.FundEntryId, row, account, direction));
            }

            var cash = Opening("Cash");
            var bank = Opening("Bank");
            var wallet = Opening("Wallet");

            var book = new FundBookDto
            {
                FromDate = from,
                ToDate = to,
                OpeningDate = openingDate,
                OpeningCash = Opening("Cash"),
                OpeningBank = Opening("Bank"),
                OpeningWallet = Opening("Wallet")
            };

            foreach (var ev in events
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.Id))
            {
                var amount = AmountOf(ev.Row);

                switch (ev.Account)
                {
                    case "Cash":
                        cash += ev.Direction * amount;
                        break;

                    case "Bank":
                        bank += ev.Direction * amount;
                        break;

                    case "Wallet":
                        wallet += ev.Direction * amount;
                        break;
                }

                if (ev.Row.BankDeposit > 0)
                    bank += ev.Row.BankDeposit;

                ev.Row.CashBalance = cash;
                ev.Row.BankBalance = bank;
                ev.Row.WalletBalance = wallet;

                if (ev.Date >= from)
                    book.Rows.Add(ev.Row);
            }

            book.TotalCashIncome = book.Rows.Sum(x => x.CashIncome);
            book.TotalBankIncome = book.Rows.Sum(x => x.BankIncome);
            book.TotalWalletIncome = book.Rows.Sum(x => x.WalletIncome);
            book.TotalExchange = book.Rows.Sum(x => x.ExchangeAmount);
            book.TotalBankDeposit = book.Rows.Sum(x => x.BankDeposit);
            book.TotalPurchaseCost = book.Rows.Sum(x => x.PurchaseCost);
            book.TotalExpense = book.Rows.Sum(x => x.Expense);

            book.ClosingCash = cash;
            book.ClosingBank = bank;
            book.ClosingWallet = wallet;

            return book;
        }

        private static void SetIncome(
            FundBookRowDto row,
            string account,
            decimal amount)
        {
            switch (account)
            {
                case "Cash":
                    row.CashIncome = amount;
                    break;

                case "Bank":
                    row.BankIncome = amount;
                    break;

                case "Wallet":
                    row.WalletIncome = amount;
                    break;

                default:
                    row.ExchangeAmount = amount;
                    break;
            }
        }

        private static decimal AmountOf(FundBookRowDto row) =>
            row.CashIncome + row.BankIncome + row.WalletIncome +
            row.BankDeposit + row.PurchaseCost + row.Expense;

        private static string AccountForMethod(string? method) =>
            method switch
            {
                "Cash" => "Cash",
                "Bank" => "Bank",
                "Cheque" => "Bank",
                "Bkash" => "Wallet",
                "Nagad" => "Wallet",
                "Tap" => "Wallet",
                "Roket" => "Wallet",
                _ => "Exchange"
            };

        // =========================================================
        // HELPERS
        // =========================================================

        private async Task<DateTime> GetOpeningDateAsync()
        {
            var dates = await _context.FundOpeningBalances
                .Select(x => x.OpeningDate)
                .ToListAsync();

            return dates.Count > 0
                ? dates.Min()
                : new DateTime(2026, 6, 1);
        }

        private DateTime ViewerToday()
        {
            var offsetMinutes = 0;

            if (Request.Cookies.TryGetValue("tz_offset", out var tzValue) &&
                int.TryParse(tzValue, out var parsedOffset))
            {
                offsetMinutes = Math.Clamp(parsedOffset, -840, 840);
            }

            return DateTime.UtcNow.AddMinutes(-offsetMinutes).Date;
        }

        private void NormalizeEntry(FundEntryDto model)
        {
            if (model.Head == "Bank Deposit")
                model.Account = "Cash";

            if (!string.IsNullOrWhiteSpace(model.Head) &&
                !Heads.Contains(model.Head))
            {
                ModelState.AddModelError(
                    nameof(model.Head), "Invalid head selected.");
            }

            if (!string.IsNullOrWhiteSpace(model.Account) &&
                !Accounts.Contains(model.Account))
            {
                ModelState.AddModelError(
                    nameof(model.Account), "Invalid account selected.");
            }
        }

        private async Task LoadEntryLookups()
        {
            ViewBag.Heads = Heads
                .Select(x => new SelectListItem(x, x))
                .ToList();

            ViewBag.Accounts = Accounts
                .Select(x => new SelectListItem(x, x))
                .ToList();

            ViewBag.OpeningDate = await GetOpeningDateAsync();
        }

        // =========================================================
        // EXCEL WORKBOOK
        // =========================================================

        private static byte[] BuildWorkbook(FundBookDto model)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Fund Book");

            ws.Cell(1, 1).Value =
                $"FUND BOOK  {model.FromDate:dd-MMM-yyyy} to {model.ToDate:dd-MMM-yyyy}";
            ws.Range(1, 1, 1, 12).Merge().Style
                .Font.SetBold(true)
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell(2, 1).Value =
                $"Opening ({model.OpeningDate:dd-MMM-yyyy}): Cash {model.OpeningCash:N2} | Bank {model.OpeningBank:N2} | Wallet {model.OpeningWallet:N2}";
            ws.Range(2, 1, 2, 12).Merge().Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            string[] headers =
            {
                "Date", "Details",
                "Cash Income", "Bank Income", "Wallet Income", "Product Exchange",
                "Bank Deposit", "Cost of Purchase", "Expense",
                "Cash Balance", "Bank Balance", "Wallet Balance"
            };

            for (var i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style
                    .Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            }

            var r = 5;

            foreach (var row in model.Rows)
            {
                ws.Cell(r, 1).Value = row.Date;
                ws.Cell(r, 1).Style.DateFormat.Format = "dd-mmm-yyyy";
                ws.Cell(r, 2).Value = row.Details;
                ws.Cell(r, 3).Value = row.CashIncome;
                ws.Cell(r, 4).Value = row.BankIncome;
                ws.Cell(r, 5).Value = row.WalletIncome;
                ws.Cell(r, 6).Value = row.ExchangeAmount;
                ws.Cell(r, 7).Value = row.BankDeposit;
                ws.Cell(r, 8).Value = row.PurchaseCost;
                ws.Cell(r, 9).Value = row.Expense;
                ws.Cell(r, 10).Value = row.CashBalance;
                ws.Cell(r, 11).Value = row.BankBalance;
                ws.Cell(r, 12).Value = row.WalletBalance;

                ws.Range(r, 3, r, 12).Style.NumberFormat.Format = "#,##0.00";

                r++;
            }

            ws.Cell(r, 2).Value = "TOTAL";
            ws.Cell(r, 3).Value = model.TotalCashIncome;
            ws.Cell(r, 4).Value = model.TotalBankIncome;
            ws.Cell(r, 5).Value = model.TotalWalletIncome;
            ws.Cell(r, 6).Value = model.TotalExchange;
            ws.Cell(r, 7).Value = model.TotalBankDeposit;
            ws.Cell(r, 8).Value = model.TotalPurchaseCost;
            ws.Cell(r, 9).Value = model.TotalExpense;
            ws.Cell(r, 10).Value = model.ClosingCash;
            ws.Cell(r, 11).Value = model.ClosingBank;
            ws.Cell(r, 12).Value = model.ClosingWallet;

            ws.Range(r, 2, r, 12).Style
                .Font.SetBold(true)
                .Border.SetTopBorder(XLBorderStyleValues.Double);
            ws.Range(r, 3, r, 12).Style.NumberFormat.Format = "#,##0.00";

            ws.Column(1).Width = 14;
            ws.Column(2).Width = 55;
            for (var c = 3; c <= 12; c++)
                ws.Column(c).Width = 15;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}