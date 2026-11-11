namespace SunwinInventory.DTOs.Sales
{
    public class SalesItemDto
    {
        public int ProductNameId { get; set; }

        public int? ColorId { get; set; }

        public decimal? Thickness { get; set; }

        public string? ProductSize { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }

        // Display purpose
        public string? ProductName { get; set; }

        public string? ColorName { get; set; }
        public string? Unit { get; set; }
    }
}