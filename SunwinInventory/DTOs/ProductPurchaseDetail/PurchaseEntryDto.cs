using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.ProductPurchaseDetail
{
    public class PurchaseEntryDto
    {
        [Required]
        public int ShipmentId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        public List<ProductPurchaseDetailDto> Items { get; set; }
            = new();
    }
}