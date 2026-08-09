using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Sales
{
    public class SalesEntryDto
    {
        [Required]
        public DateTime SalesDate { get; set; } = DateTime.Now;

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        public string? SalesType { get; set; }

        public string? ReceiveMethod { get; set; }

        public decimal ReceiveAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal Vat { get; set; }

        public decimal Ait { get; set; }

        public string? Note { get; set; }

        public List<SalesItemDto> Items { get; set; } = new();
    }
}