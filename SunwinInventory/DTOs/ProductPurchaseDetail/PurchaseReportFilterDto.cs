namespace SunwinInventory.DTOs.ProductPurchaseDetail
{
    public class PurchaseReportFilterDto
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? SupplierId { get; set; }

        public int? WarehouseId { get; set; }

        public int? ShipmentId { get; set; }

        public int? ProductNameId { get; set; }
    }
}