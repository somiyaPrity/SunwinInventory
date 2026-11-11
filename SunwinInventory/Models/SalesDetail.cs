using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

public partial class SalesDetail
{
    [Key]
    public int SalesDetailId { get; set; }

    public int SalesInvoiceId { get; set; }

    public int PurchaseDetailId { get; set; }

    public int ShipmentId { get; set; }

    public int ProductNameId { get; set; }

    public int? ColorId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Thickness { get; set; }

    [StringLength(100)]
    public string? ProductSize { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? WeightKg { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? LineTotal { get; set; }

    [StringLength(20)]
    public string? Unit { get; set; }
}
