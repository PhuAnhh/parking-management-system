using System.Text.Json.Serialization;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Application.Models
{
    public class RevenueReportDto
    {
        public int Id { get; set; }

        public int CardGroupId { get; set; }

        public int ExitCount { get; set; }

        public decimal Revenue { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
