using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.CustomerPayment;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class CustomerPaymentRepository : ICustomerPaymentRepository
    {
        private readonly SunwinInventoryContext _context;

        public CustomerPaymentRepository(
            SunwinInventoryContext context)
        {
            _context = context;
        }

        //=====================================================
        // GET ALL PAYMENTS
        //=====================================================

        public async Task<List<CustomerPaymentDto>> GetAllAsync()
        {
            return await
            (
                from p in _context.CustomerPayments

                join c in _context.Customers
                    on p.CustomerId equals c.CustomerId

                join i in _context.SalesInvoices
                    on p.SalesInvoiceId equals i.SalesInvoiceId

                orderby p.CustomerPaymentId descending

                select new CustomerPaymentDto
                {
                    CustomerPaymentId = p.CustomerPaymentId,

                    CustomerId = p.CustomerId,

                    SalesInvoiceId = p.SalesInvoiceId,

                    CustomerName = c.CustomerName,

                    InvoiceNo = i.InvoiceNo,

                    PaymentAmount = p.PaymentAmount,

                    PaymentDate = p.PaymentDate,

                    PaymentMethod = p.PaymentMethod,

                    Remarks = p.Remarks,

                    InvoiceTotal = i.TotalAmount ?? 0,

                    InvoicePaidAmount = i.PaidAmount ?? 0,

                    InvoiceDueAmount = i.DueAmount ?? 0
                }
            ).ToListAsync();
        }
        public async Task<int?> AddPaymentAsync(CustomerPaymentDto model)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var invoice = await _context.SalesInvoices
                    .FirstOrDefaultAsync(x =>
                        x.SalesInvoiceId == model.SalesInvoiceId);

                if (invoice == null)
                    return null;

                if (invoice.CustomerId != model.CustomerId)
                    return null;

                decimal totalAmount = invoice.TotalAmount ?? 0;
                decimal currentPaid = invoice.PaidAmount ?? 0;
                decimal currentDue = invoice.DueAmount ?? 0;

                if (model.PaymentAmount <= 0)
                    return null;

                if (model.PaymentAmount > currentDue)
                    return null;

                var payment = new CustomerPayment
                {
                    CustomerId = model.CustomerId,

                    SalesInvoiceId = model.SalesInvoiceId,

                    PaymentAmount = model.PaymentAmount,

                    PaymentDate = model.PaymentDate,

                    PaymentMethod = model.PaymentMethod,

                    BankName = model.BankName,

                    TransectionCode = model.TransactionCode,

                    Remarks = model.Remarks,

                    CreatedBy = "Admin",

                    CreatedDate = DateTime.Now
                };

                _context.CustomerPayments.Add(payment);

                decimal newPaid =
                    currentPaid + model.PaymentAmount;

                decimal newDue =
                    Math.Max(0, totalAmount - newPaid);

                invoice.PaidAmount = newPaid;

                invoice.DueAmount = newDue;

                invoice.UpdatedBy = "Admin";

                invoice.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return payment.CustomerPaymentId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //=====================================================
        // GET PAYMENT BY ID
        //=====================================================

        public async Task<CustomerPaymentDto?> GetByIdAsync(int id)
        {
            return await
            (
                from p in _context.CustomerPayments

                join c in _context.Customers
                    on p.CustomerId equals c.CustomerId

                join i in _context.SalesInvoices
                    on p.SalesInvoiceId equals i.SalesInvoiceId

                where p.CustomerPaymentId == id

                select new CustomerPaymentDto
                {
                    CustomerPaymentId = p.CustomerPaymentId,

                    CustomerId = p.CustomerId,

                    SalesInvoiceId = p.SalesInvoiceId,

                    CustomerName = c.CustomerName,

                    InvoiceNo = i.InvoiceNo,

                    PaymentAmount = p.PaymentAmount,

                    PaymentDate = p.PaymentDate,

                    PaymentMethod = p.PaymentMethod,

                    Remarks = p.Remarks,

                    InvoiceTotal = i.TotalAmount ?? 0,

                    InvoicePaidAmount = i.PaidAmount ?? 0,

                    InvoiceDueAmount = i.DueAmount ?? 0
                }
            ).FirstOrDefaultAsync();
        }

        //=====================================================
        // CREATE PAYMENT + UPDATE INVOICE
        //=====================================================

        public async Task<bool> CreateAsync(
            CustomerPaymentDto model)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Get invoice
                var invoice =
                    await _context.SalesInvoices
                        .FirstOrDefaultAsync(x =>
                            x.SalesInvoiceId ==
                            model.SalesInvoiceId);

                if (invoice == null)
                    return false;

                // Validate customer
                if (invoice.CustomerId != model.CustomerId)
                    return false;

                // Current invoice values
                decimal totalAmount =
                    invoice.TotalAmount ?? 0;

                decimal currentPaid =
                    invoice.PaidAmount ?? 0;

                decimal currentDue =
                    invoice.DueAmount ?? 0;

                // Validate payment
                if (model.PaymentAmount <= 0)
                    return false;

                if (model.PaymentAmount > currentDue)
                    return false;

                // Create payment
                var payment = new CustomerPayment
                {
                    CustomerId = model.CustomerId,

                    SalesInvoiceId =
                        model.SalesInvoiceId,

                    PaymentAmount =
                        model.PaymentAmount,

                    PaymentDate =
                        model.PaymentDate,

                    PaymentMethod =
                        model.PaymentMethod,

                    Remarks =
                        model.Remarks,

                    CreatedBy = "Admin",

                    CreatedDate =
                        DateTime.Now
                };

                _context.CustomerPayments.Add(payment);

                // Update invoice
                decimal newPaid =
                    currentPaid +
                    model.PaymentAmount;

                decimal newDue =
                    totalAmount -
                    newPaid;

                if (newDue < 0)
                    newDue = 0;

                invoice.PaidAmount = newPaid;

                invoice.DueAmount = newDue;

                invoice.UpdatedBy = "Admin";

                invoice.UpdatedDate =
                    DateTime.Now;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        //=====================================================
        // GET PAYMENTS BY INVOICE
        //=====================================================

        public async Task<List<CustomerPaymentDto>>
            GetByInvoiceAsync(int salesInvoiceId)
        {
            return await
            (
                from p in _context.CustomerPayments

                join c in _context.Customers
                    on p.CustomerId equals c.CustomerId

                join i in _context.SalesInvoices
                    on p.SalesInvoiceId equals i.SalesInvoiceId

                where p.SalesInvoiceId == salesInvoiceId

                orderby p.PaymentDate descending,
                        p.CustomerPaymentId descending

                select new CustomerPaymentDto
                {
                    CustomerPaymentId =
                        p.CustomerPaymentId,

                    CustomerId =
                        p.CustomerId,

                    SalesInvoiceId =
                        p.SalesInvoiceId,

                    CustomerName =
                        c.CustomerName,

                    InvoiceNo =
                        i.InvoiceNo,

                    PaymentAmount =
                        p.PaymentAmount,

                    PaymentDate =
                        p.PaymentDate,

                    PaymentMethod =
                        p.PaymentMethod,

                    Remarks =
                        p.Remarks,

                    InvoiceTotal =
                        i.TotalAmount ?? 0,

                    InvoicePaidAmount =
                        i.PaidAmount ?? 0,

                    InvoiceDueAmount =
                        i.DueAmount ?? 0
                }
            ).ToListAsync();
        }

        //=====================================================
        // GET PAYMENTS BY CUSTOMER
        //=====================================================

        public async Task<List<CustomerPaymentDto>>
            GetByCustomerAsync(int customerId)
        {
            return await
            (
                from p in _context.CustomerPayments

                join c in _context.Customers
                    on p.CustomerId equals c.CustomerId

                join i in _context.SalesInvoices
                    on p.SalesInvoiceId equals i.SalesInvoiceId

                where p.CustomerId == customerId

                orderby p.PaymentDate descending,
                        p.CustomerPaymentId descending

                select new CustomerPaymentDto
                {
                    CustomerPaymentId =
                        p.CustomerPaymentId,

                    CustomerId =
                        p.CustomerId,

                    SalesInvoiceId =
                        p.SalesInvoiceId,

                    CustomerName =
                        c.CustomerName,

                    InvoiceNo =
                        i.InvoiceNo,

                    PaymentAmount =
                        p.PaymentAmount,

                    PaymentDate =
                        p.PaymentDate,

                    PaymentMethod =
                        p.PaymentMethod,

                    Remarks =
                        p.Remarks,

                    InvoiceTotal =
                        i.TotalAmount ?? 0,

                    InvoicePaidAmount =
                        i.PaidAmount ?? 0,

                    InvoiceDueAmount =
                        i.DueAmount ?? 0
                }
            ).ToListAsync();
        }

        //=====================================================
        // GET ALL CUSTOMER INVOICES
        //=====================================================

        public async Task<List<CustomerPaymentDto>>
            GetCustomerInvoicesAsync(int customerId)
        {
            return await
            (
                from invoice in _context.SalesInvoices

                join customer in _context.Customers
                    on invoice.CustomerId equals customer.CustomerId

                where invoice.CustomerId == customerId

                orderby invoice.InvoiceDate descending

                select new CustomerPaymentDto
                {
                    CustomerId =
                        invoice.CustomerId,

                    SalesInvoiceId =
                        invoice.SalesInvoiceId,

                    InvoiceNo =
                        invoice.InvoiceNo,

                    CustomerName =
                        customer.CustomerName,

                    InvoiceTotal =
                        invoice.TotalAmount ?? 0,

                    InvoicePaidAmount =
                        invoice.PaidAmount ?? 0,

                    InvoiceDueAmount =
                        invoice.DueAmount ?? 0
                }
            ).ToListAsync();
        }

        //=====================================================
        // GET INVOICE INFO
        //=====================================================

        public async Task<CustomerPaymentDto?>
            GetInvoiceInfoAsync(int salesInvoiceId)
        {
            return await
            (
                from invoice in _context.SalesInvoices

                join customer in _context.Customers
                    on invoice.CustomerId equals customer.CustomerId

                where invoice.SalesInvoiceId ==
                      salesInvoiceId

                select new CustomerPaymentDto
                {
                    CustomerId =
                        invoice.CustomerId,

                    SalesInvoiceId =
                        invoice.SalesInvoiceId,

                    InvoiceNo =
                        invoice.InvoiceNo,

                    CustomerName =
                        customer.CustomerName,

                    InvoiceTotal =
                        invoice.TotalAmount ?? 0,

                    InvoicePaidAmount =
                        invoice.PaidAmount ?? 0,

                    InvoiceDueAmount =
                        invoice.DueAmount ?? 0
                }
            ).FirstOrDefaultAsync();
        }

        //=====================================================
        // DELETE PAYMENT + RESTORE INVOICE DUE
        //=====================================================

        public async Task DeleteAsync(int id)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var payment =
                    await _context.CustomerPayments
                        .FirstOrDefaultAsync(x =>
                            x.CustomerPaymentId == id);

                if (payment == null)
                    return;

                var invoice =
                    await _context.SalesInvoices
                        .FirstOrDefaultAsync(x =>
                            x.SalesInvoiceId ==
                            payment.SalesInvoiceId);

                if (invoice != null)
                {
                    decimal currentPaid =
                        invoice.PaidAmount ?? 0;

                    decimal newPaid =
                        currentPaid -
                        payment.PaymentAmount;

                    if (newPaid < 0)
                        newPaid = 0;

                    decimal totalAmount =
                        invoice.TotalAmount ?? 0;

                    decimal newDue =
                        totalAmount -
                        newPaid;

                    if (newDue < 0)
                        newDue = 0;

                    invoice.PaidAmount =
                        newPaid;

                    invoice.DueAmount =
                        newDue;

                    invoice.UpdatedBy =
                        "Admin";

                    invoice.UpdatedDate =
                        DateTime.Now;
                }

                _context.CustomerPayments
                    .Remove(payment);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
        public async Task<List<CustomerDueDto>> GetCustomerDueReportAsync()
        {
            return await
                (
                    from c in _context.Customers

                    join invoice in _context.SalesInvoices
                        on c.CustomerId equals invoice.CustomerId
                        into invoiceGroup

                    let totalInvoice =
                        invoiceGroup.Sum(x => x.TotalAmount ?? 0)

                    let totalPaid =
                        invoiceGroup.Sum(x => x.PaidAmount ?? 0)

                    let invoiceDue =
                        invoiceGroup.Sum(x => x.DueAmount ?? 0)

                    let totalDue =
                        c.PreviousDue + invoiceDue

                    where totalDue > 0

                    select new CustomerDueDto
                    {
                        CustomerId = c.CustomerId,

                        CustomerCode = c.CustomerCode,

                        CustomerName = c.CustomerName,

                        ContactPerson = c.ContactPerson,

                        Mobile = c.Mobile,

                        Address = c.Address,

                        PreviousDue = c.PreviousDue,

                        TotalInvoiceAmount = totalInvoice,

                        TotalPaidAmount = totalPaid,

                        TotalDueAmount = totalDue
                    }
                )
                .OrderByDescending(x => x.TotalDueAmount)
                .ToListAsync();
        }
    }
}