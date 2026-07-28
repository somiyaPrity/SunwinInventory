using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.ProductPurchaseDetail
{
    public class ProductPurchaseDetailDto
    {
        public string? ProductName { get; set; }

        public string? ColorName { get; set; }
        public decimal? CostPricePcsTk { get; set; }

        public decimal? CostPricePcsRmb { get; set; }

       
        public int PurchaseDetailId { get; set; }

        [Required]
        public int ShipmentId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductNameId { get; set; }

        [Required]
        public int ColorId { get; set; }

        public decimal? Thickness { get; set; }

        public string? ProductSize { get; set; }

        [Required]
        public int InitialQuantity { get; set; }

        public decimal? UnitWeightKg { get; set; }

        public decimal? TotalWeightKg { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? UnitPriceRmb { get; set; }

        public decimal? UnitPriceTk { get; set; }

        public decimal? ImportTaxPerKgTk { get; set; }

        public decimal? ImportTaxPerKgRmb { get; set; }
        public decimal? CostPricePerPieceTk { get; set; }
        public decimal? CostPriceKgTk { get; set; }

        public decimal? CostPriceLbsTk { get; set; }

        public decimal? CostPriceKgRmb { get; set; }

        public decimal? CostPriceLbsRmb { get; set; }

        public decimal? TotalAmountWithoutTaxTk { get; set; }

        public decimal? TotalAmountIncludingTaxTk { get; set; }

        public decimal? TotalAmountWithoutTaxRmb { get; set; }

        public decimal? TotalAmountIncludingTaxRmb { get; set; }

        public string? Remarks { get; set; }
    }
}