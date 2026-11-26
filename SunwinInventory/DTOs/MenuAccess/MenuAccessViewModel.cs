using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.Role;

namespace SunwinInventory.DTOs.MenuAccess
{
    public class MenuAccessViewModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public bool IsAdministrator { get; set; }

        public List<MenuAccessDto> Menus { get; set; } = new();

        public List<SelectListItem> Roles { get; set; } = new();
    }
}
