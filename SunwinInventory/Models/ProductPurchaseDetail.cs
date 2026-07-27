using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

public partial class ProductPurchaseDetail
{
    [Key]
    public int PurchaseDetailId { get; set; }

    public int ShipmentId { get; set; }

    public int SupplierId { get; set; }

    public int WarehouseId { get; set; }

    public int ProductNameId { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal? CostPricePerPieceTk { get; set; }
    public int ColorId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Thickness { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? ProductSize { get; set; }

    public int InitialQuantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? UnitWeightKg { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? TotalWeightKg { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? ExchangeRate { get; set; }

    [Column("UnitPriceRMB", TypeName = "decimal(18, 4)")]
    public decimal? UnitPriceRmb { get; set; }

    [Column("UnitPriceTK", TypeName = "decimal(18, 4)")]
    public decimal? UnitPriceTk { get; set; }

    [Column("ImportTaxPerKgTK", TypeName = "decimal(18, 4)")]
    public decimal? ImportTaxPerKgTk { get; set; }

    [Column("ImportTaxPerKgRMB", TypeName = "decimal(18, 4)")]
    public decimal? ImportTaxPerKgRmb { get; set; }

    [Column("CostPriceKgTK", TypeName = "decimal(18, 4)")]
    public decimal? CostPriceKgTk { get; set; }

    [Column("CostPriceLbsTK", TypeName = "decimal(18, 4)")]
    public decimal? CostPriceLbsTk { get; set; }

    [Column("CostPriceKgRMB", TypeName = "decimal(18, 4)")]
    public decimal? CostPriceKgRmb { get; set; }

    [Column("CostPriceLbsRMB", TypeName = "decimal(18, 4)")]
    public decimal? CostPriceLbsRmb { get; set; }

    [Column("TotalAmountWithoutTaxTK", TypeName = "decimal(18, 2)")]
    public decimal? TotalAmountWithoutTaxTk { get; set; }

    [Column("TotalAmountIncludingTaxTK", TypeName = "decimal(18, 2)")]
    public decimal? TotalAmountIncludingTaxTk { get; set; }

    [Column("TotalAmountWithoutTaxRMB", TypeName = "decimal(18, 2)")]
    public decimal? TotalAmountWithoutTaxRmb { get; set; }

    [Column("TotalAmountIncludingTaxRMB", TypeName = "decimal(18, 2)")]
    public decimal? TotalAmountIncludingTaxRmb { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }
}
