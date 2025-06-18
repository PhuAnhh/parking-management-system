namespace Final_year_Project.Application.Models
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class UpdateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }
}
