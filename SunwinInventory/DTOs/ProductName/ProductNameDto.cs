using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.ProductName
{
    public class ProductNameDto
    {
        public int ProductNameId { get; set; }

        //[Required(ErrorMessage = "Product Code is required.")]
        [Display(Name = "Product Code")]
        public string? ProductCode { get; set; } 

        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}