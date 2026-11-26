using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

[Index("InvoiceNo", Name = "UQ__SalesInv__D796B227F37409A2", IsUnique = true)]
public partial class SalesInvoice
{
    [Key]
    public int SalesInvoiceId { get; set; }

    [StringLength(50)]
    public string InvoiceNo { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime InvoiceDate { get; set; }

    public int CustomerId { get; set; }

    public int WarehouseId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Vat { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Ait { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PaidAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DueAmount { get; set; }

    [StringLength(30)]
    public string? ReceiveMethod { get; set; }

    [StringLength(30)]
    public string? BankName { get; set; }

    [Column("Transection_code")]
    [StringLength(30)]
    public string? TransectionCode { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CuttingCharge { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DeliveryCharge { get; set; }
}
