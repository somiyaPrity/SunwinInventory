using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

[Index("CategoryName", Name = "IX_ProductCategories_CategoryName", IsUnique = true)]
public partial class ProductCategory
{
    [Key]
    public int CategoryId { get; set; }

    [StringLength(100)]
    public string CategoryName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }
}
