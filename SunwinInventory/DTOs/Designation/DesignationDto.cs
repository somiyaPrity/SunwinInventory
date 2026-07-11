using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Designation
{
    public class DesignationDto
    {
        public int DesignationId { get; set; }

        [Required]
        [StringLength(100)]
        public string DesignationName { get; set; } = string.Empty;

        public int? ReportTo { get; set; }

        public string? ReportToName { get; set; }

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}