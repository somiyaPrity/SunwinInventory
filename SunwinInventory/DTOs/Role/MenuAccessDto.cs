namespace SunwinInventory.DTOs.Role
{
    public class MenuAccessDto
    {
        public string MenuKey { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public bool CanView { get; set; }

        public bool CanCreate { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }
    }
}
