using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

[Index("CustomerId", Name = "IX_CustomerPayments_CustomerId")]
[Index("PaymentDate", Name = "IX_CustomerPayments_PaymentDate")]
[Index("SalesInvoiceId", Name = "IX_CustomerPayments_SalesInvoiceId")]
public partial class CustomerPayment
{
    [Key]
    public int CustomerPaymentId { get; set; }

    public int CustomerId { get; set; }

    public int SalesInvoiceId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PaymentAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? PaymentMethod { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Remarks { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }
}
