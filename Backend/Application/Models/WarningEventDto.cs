using System.Text.Json.Serialization;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Application.Models
{
    public class WarningEventDto
    {
        public int Id { get; set; }

        public string? PlateNumber { get; set; }

        public int LaneId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WarningType WarningType { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class CreateWarningEventDto
    {
        public string? PlateNumber { get; set; }

        public int LaneId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WarningType WarningType { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ImageUrl { get; set; }
    }
}
