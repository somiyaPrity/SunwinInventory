using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

[Table("SalesHeader")]
public partial class SalesHeader
{
    [Key]
    public int SalesId { get; set; }

    [StringLength(30)]
    public string InvoiceNo { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime SalesDate { get; set; }

    public int CustomerId { get; set; }

    public int WarehouseId { get; set; }

    [StringLength(20)]
    public string? SalesType { get; set; }

    [StringLength(30)]
    public string? ReceiveMethod { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ReceiveAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Vat { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Ait { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DueAmount { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }
}
