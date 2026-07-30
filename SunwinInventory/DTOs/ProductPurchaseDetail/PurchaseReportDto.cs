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

        public decimal TotalWeightKg { get; set; }

        public decimal UnitPriceTk { get; set; }

        public decimal TotalAmountIncludingTaxTk { get; set; }
    }
}