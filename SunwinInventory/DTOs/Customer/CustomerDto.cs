using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Customer
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }

       
        public string? CustomerCode { get; set; }

        [Required]
        public string CustomerName { get; set; }

        public string? ContactPerson { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}