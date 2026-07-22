using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.ProductPurchaseDetail
{
    public class PurchaseEntryDto
    {
        [Required]
        public int ShipmentId { get; set; }

        public List<ProductPurchaseDetailDto> Items { get; set; }
            = new List<ProductPurchaseDetailDto>();
    }
}