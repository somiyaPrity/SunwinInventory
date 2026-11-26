namespace SunwinInventory.DTOs.StockReport
{
    public class StockMovementDto
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public DateTime MonthStart => new(Year, Month, 1);

        public int DaysInMonth => DateTime.DaysInMonth(Year, Month);

        public List<StockWarehouseColumnDto> Warehouses { get; set; } = new();

        public List<StockMovementRowDto> Rows { get; set; } = new();

        public StockMovementTotalsDto Totals { get; set; } = new();

        public string Title =>
            $"STOCK MOVEMENT REPORT {MonthStart:MMMM yyyy}".ToUpper();
    }

    public class StockMovementRowDto
    {
        public int ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = null!;

        public DateTime? ArrivalDate { get; set; }

        public string ProductName { get; set; } = null!;

        public string? ColorName { get; set; }

        public decimal? Thickness { get; set; }

        public string? ProductSize { get; set; }

        public decimal UnitWeightKg { get; set; }

        public int OpeningQty { get; set; }

        public int ArrivalQty { get; set; }

        public int SalesQty { get; set; }

        public int ClosingQty => OpeningQty + ArrivalQty - SalesQty;

        public decimal WeightPerPcsLbs => UnitWeightKg * 2.20462m;

        public decimal TotalWeightKg => ClosingQty * UnitWeightKg;

        public decimal TotalWeightLbs => TotalWeightKg * 2.20462m;

        public Dictionary<int, int> ClosingByWarehouse { get; set; } = new();

        public Dictionary<int, int> DailySales { get; set; } = new();
    }

    public class StockMovementTotalsDto
    {
        public int OpeningQty { get; set; }

        public int ArrivalQty { get; set; }

        public int SalesQty { get; set; }

        public int ClosingQty { get; set; }

        public decimal TotalWeightKg { get; set; }

        public decimal TotalWeightLbs => TotalWeightKg * 2.20462m;

        public Dictionary<int, int> ClosingByWarehouse { get; set; } = new();

        public Dictionary<int, int> DailySales { get; set; } = new();
    }
}