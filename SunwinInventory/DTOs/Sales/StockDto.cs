namespace SunwinInventory.DTOs.Sales
{
    public class StockDto
    {
        public int StockLedgerId { get; set; }

        public int PurchaseDetailId { get; set; }

        public int ShipmentId { get; set; }

        public string? ShipmentNo { get; set; }

        public int AvailableQty { get; set; }

        public decimal AvailableWeight { get; set; }

        public DateTime PurchaseDate { get; set; }
    }
}