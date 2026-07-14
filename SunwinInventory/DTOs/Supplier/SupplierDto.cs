using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Supplier
{
    public class SupplierDto
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier Code is required.")]
        [Display(Name = "Supplier Code")]
        public string SupplierCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Supplier Name is required.")]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}