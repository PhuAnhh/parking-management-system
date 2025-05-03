namespace Final_year_Project.Application.Models
{
    public class ComputerDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public int? GateId { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CreateComputerDto
    {
        public string Name { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public int GateId { get; set; }

        public bool Status { get; set; }
    }

    public class UpdateComputerDto
    {
        public string Name { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public int GateId { get; set; }

        public bool Status { get; set; }
    }
}
