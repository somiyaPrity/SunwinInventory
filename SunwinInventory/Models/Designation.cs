using System;
using System.Collections.Generic;

namespace SunwinInventory.Models;

public partial class Designation
{
    public int DesignationId { get; set; }

    public string DesignationName { get; set; } = null!;

    public int? ReportTo { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
