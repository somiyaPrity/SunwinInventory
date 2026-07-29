using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.ProductPurchaseDetail
{
  
    public class PurchaseEntryDto
    {
        public int ShipmentId { get; set; }

        public int SupplierId { get; set; }

        public int WarehouseId { get; set; }

        public IFormFile? ExcelFile { get; set; }

        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        public int ShipmentId { get; set; }

        public int SupplierId { get; set; }

        public int WarehouseId { get; set; }

        public int ProductNameId { get; set; }

        public string? ProductName { get; set; }

        public int ColorId { get; set; }

        public string? ColorName { get; set; }

        public decimal Thickness { get; set; }

        public string? ProductSize { get; set; }

        public int InitialQuantity { get; set; }

        public decimal UnitWeightKg { get; set; }

        public decimal TotalWeightKg { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal UnitPriceRmb { get; set; }

        public decimal UnitPriceTk { get; set; }

        public decimal ImportTaxPerKgTk { get; set; }

        public decimal ImportTaxPerKgRmb { get; set; }

        public decimal CostPriceKgTk { get; set; }

        public decimal CostPriceLbsTk { get; set; }

        public decimal CostPricePerPieceTk { get; set; }

        public decimal CostPriceKgRmb { get; set; }

        public decimal CostPriceLbsRmb { get; set; }

        public decimal CostPricePcsRmb { get; set; }

        public decimal TotalAmountWithoutTaxTk { get; set; }

        public decimal TotalAmountIncludingTaxTk { get; set; }

        public decimal TotalAmountWithoutTaxRmb { get; set; }

        public decimal TotalAmountIncludingTaxRmb { get; set; }

        public string? Remarks { get; set; }
    }
}