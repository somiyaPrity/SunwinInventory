using System;
using System.Collections.Generic;

namespace SunwinInventory.Models;

public partial class ProductName
{
    public int ProductNameId { get; set; }

    public string? ProductCode { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
