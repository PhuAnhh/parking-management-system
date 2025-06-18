namespace Final_year_Project.Application.Models
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
    }
}
