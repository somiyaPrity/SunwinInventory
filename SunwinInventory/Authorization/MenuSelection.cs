using SunwinInventory.DTOs.Role;

namespace SunwinInventory.Authorization
{
    public static class MenuSelection
    {
        public const string Separator = "|";

        // Checkbox values look like "warehouse|view" / "warehouse|delete".
        public static List<MenuAccessDto> Parse(IEnumerable<string>? selections)
        {
            var selected = new HashSet<string>(
                selections ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            return MenuCatalog.Menus
                .Select(menu => new MenuAccessDto
                {
                    MenuKey = menu.Key,
                    Title = menu.Title,
                    Group = menu.Group,
                    Icon = menu.Icon,
                    CanView = selected.Contains(menu.Key + Separator + "view"),
                    CanCreate = selected.Contains(menu.Key + Separator + "create"),
                    CanEdit = selected.Contains(menu.Key + Separator + "edit"),
                    CanDelete = selected.Contains(menu.Key + Separator + "delete")
                })
                .ToList();
        }
    }
}
