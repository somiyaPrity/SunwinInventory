using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunwinInventory.Models;

public partial class FundEntry
{
    [Key]
    public int FundEntryId { get; set; }

    [Column(TypeName = "date")]
    public DateTime EntryDate { get; set; }

    [StringLength(50)]
    public string Head { get; set; } = null!;

    [StringLength(20)]
    public string Account { get; set; } = null!;

    [StringLength(200)]
    public string? Details { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [StringLength(50)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }
}