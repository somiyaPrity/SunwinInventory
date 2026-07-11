using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Account
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}