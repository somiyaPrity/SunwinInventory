namespace SunwinInventory.DTOs.ProductPurchaseDetail
{
    public class PurchaseReportDto
    {
        public int ProductPurchaseDetailId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ShipmentNo { get; set; }

        public string SupplierName { get; set; }

        public string WarehouseName { get; set; }

        public string ProductName { get; set; }

        public string ColorName { get; set; }

        public decimal Thickness { get; set; }

        public string ProductSize { get; set; }

        public int InitialQuantity { get; set; }

        public decimal UnitWeightKg { get; set; }

        public decimal TotalWeightKg { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal UnitPriceTk { get; set; }

        public decimal UnitPriceRmb { get; set; }

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