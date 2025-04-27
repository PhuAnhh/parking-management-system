using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Models
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public int? CustomerGroupId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCustomerDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public int? CustomerGroupId { get; set; }
    }

    public class UpdateCustomerDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public int? CustomerGroupId { get; set; }
    }
}
