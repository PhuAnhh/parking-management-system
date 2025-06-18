namespace Final_year_Project.Application.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string Role{ get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
