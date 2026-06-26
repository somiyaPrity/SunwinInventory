using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Warehouse
{
    public class CreateWarehouseDto
    {
        [Required]
        [StringLength(100)]
        public string WarehouseName { get; set; } = "";

        [StringLength(500)]
        public string? Address { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }
    }
}