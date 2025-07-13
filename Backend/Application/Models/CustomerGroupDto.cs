using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Models
{
    public class CustomerGroupDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int CardCount { get; set; }
    }

    public class CreateCustomerGroupDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;
    }

    public class UpdateCustomerGroupDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}
