using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunwinInventory.Models;

[Table("RoleMenuPermissions")]
public partial class RoleMenuPermission
{
    [Key]
    public int Id { get; set; }

    public int RoleId { get; set; }

    [StringLength(50)]
    public string MenuKey { get; set; } = null!;

    public bool CanView { get; set; }

    public bool CanCreate { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }
}
