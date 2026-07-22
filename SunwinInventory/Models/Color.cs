using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

public partial class Color
{
    [Key]
    public int ColorId { get; set; }

    [StringLength(100)]
    public string ColorName { get; set; } = null!;

    public bool IsActive { get; set; }
}
