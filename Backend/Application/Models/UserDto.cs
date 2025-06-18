namespace Final_year_Project.Application.Models
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Name { get; set; }

        public int RoleId { get; set; }

        public bool Status { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Name { get; set; }

        public int RoleId { get; set; }

        public bool Status { get; set; }
    }

    public class UpdateUserDto
    {
        public string Username { get; set; } = null!;

        public string? Name { get; set; }

        public int RoleId { get; set; }

        public bool Status { get; set; }
    }
}
