using System;
using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Fund
{
    public class FundEntryDto
    {
        public int FundEntryId { get; set; }

        [Required(ErrorMessage = "Entry date is required.")]
        [DataType(DataType.Date)]
        public DateTime? EntryDate { get; set; }

        [Required(ErrorMessage = "Please select a head.")]
        [StringLength(50)]
        public string Head { get; set; } = "";

        [Required(ErrorMessage = "Please select an account.")]
        [StringLength(20)]
        public string Account { get; set; } = "";

        [StringLength(200)]
        public string? Details { get; set; }

        [Range(0.01, 9999999999, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}