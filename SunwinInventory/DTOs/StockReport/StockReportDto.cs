namespace SunwinInventory.DTOs.StockReport
{
    public class StockReportDto
    {
        public DateTime AsOf { get; set; }

        public int ProductCount { get; set; }

        public List<StockWarehouseColumnDto> Warehouses { get; set; } = new();

        public List<StockReportRowDto> Rows { get; set; } = new();

        public StockReportTotalsDto Totals { get; set; } = new();

        public string Title =>
            $"CURRENT STOCK REPORT AS ON {AsOf:dd-MMM-yyyy}".ToUpper();
    }

    public class StockWarehouseColumnDto
    {
        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; } = null!;
    }

    public class StockReportRowDto
    {
        public int ShipmentId { get; set; }

        public DateTime? ArrivalDate { get; set; }

        public string ShipmentNo { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public string? ColorName { get; set; }

        public decimal? Thickness { get; set; }

        public string? ProductSize { get; set; }

        public int Quantity { get; set; }

        public decimal UnitWeightKg { get; set; }

        public decimal WeightPerPcsLbs => UnitWeightKg * 2.20462m;

        public decimal TotalWeightKg { get; set; }

        public decimal TotalWeightLbs => TotalWeightKg * 2.20462m;

        public Dictionary<int, int> QuantityByWarehouse { get; set; } = new();
    }

    public class StockReportTotalsDto
    {
        public int Quantity { get; set; }

        public decimal TotalWeightKg { get; set; }

        public decimal TotalWeightLbs => TotalWeightKg * 2.20462m;

        public Dictionary<int, int> QuantityByWarehouse { get; set; } = new();
    }
}
