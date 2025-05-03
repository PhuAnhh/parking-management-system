namespace Final_year_Project.Application.Models
{
    public class GateDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CreateGateDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public bool Status { get; set; }
    }

    public class UpdateGateDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public bool Status { get; set; }
    }
}
