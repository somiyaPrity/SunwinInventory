using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunwinInventory.Models;

public partial class FundOpeningBalance
{
    [Key]
    public int FundOpeningBalanceId { get; set; }

    [StringLength(20)]
    public string Account { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime OpeningDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }
}