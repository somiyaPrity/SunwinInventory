using System;
using System.Collections.Generic;

namespace SunwinInventory.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int RoleId { get; set; }

    public int DesignationId { get; set; }

    public int WarehouseId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool IsLocked { get; set; }

    public int FailedLoginCount { get; set; }

    public bool MustChangePassword { get; set; }
}
