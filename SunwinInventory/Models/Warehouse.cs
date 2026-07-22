using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

[Index("WarehouseName", Name = "UQ_Warehouses_WarehouseName", IsUnique = true)]
public partial class Warehouse
{
    [Key]
    public int WarehouseId { get; set; }

    [StringLength(100)]
    public string WarehouseName { get; set; } = null!;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }
}
