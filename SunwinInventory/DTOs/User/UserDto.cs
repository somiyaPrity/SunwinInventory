using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.User
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        public string? Password { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int DesignationId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        public string? RoleName { get; set; }

        public string? DesignationName { get; set; }

        public string? WarehouseName { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}