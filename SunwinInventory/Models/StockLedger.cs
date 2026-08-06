using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

[Table("StockLedger")]
public partial class StockLedger
{
    [Key]
    public int StockLedgerId { get; set; }

    public int PurchaseDetailId { get; set; }

    public int WarehouseId { get; set; }

    public int ProductNameId { get; set; }

    public int? ColorId { get; set; }

    public int AvailableQty { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal AvailableWeight { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PurchaseDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? UnitCost { get; set; }
}
