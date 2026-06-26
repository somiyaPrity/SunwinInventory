namespace SunwinInventory.DTOs.Warehouse
{
    public class WarehouseDto
    {
        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; } = "";

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? ContactPerson { get; set; }

        public bool IsActive { get; set; }
    }
}