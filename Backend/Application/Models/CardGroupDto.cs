using System.Text.Json.Serialization;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Application.Models
{
    public class CardGroupDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardGroupType Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardGroupVehicleType VehicleType { get; set; }

        public int? FreeMinutes { get; set; }

        public int? FirstBlockMinutes { get; set; }

        public decimal? FirstBlockPrice { get; set; }

        public int? NextBlockMinutes { get; set; }

        public decimal? NextBlockPrice { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<int> LaneIds { get; set; }
    }

    public class CreateCardGroupDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardGroupType Type { get; set; } 

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardGroupVehicleType VehicleType { get; set; } 

        public int? FreeMinutes { get; set; }

        public int? FirstBlockMinutes { get; set; }

        public decimal? FirstBlockPrice { get; set; }

        public int? NextBlockMinutes { get; set; }

        public decimal? NextBlockPrice { get; set; }

        public bool Status { get; set; }

        public List<int> LaneIds { get; set; } = new List<int>();
    }

    public class UpdateCardGroupDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardGroupType Type { get; set; } 

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardGroupVehicleType VehicleType { get; set; } 

        public int? FreeMinutes { get; set; }

        public int? FirstBlockMinutes { get; set; }

        public decimal? FirstBlockPrice { get; set; }

        public int? NextBlockMinutes { get; set; }

        public decimal? NextBlockPrice { get; set; }

        public bool Status { get; set; }

        public List<int> LaneIds { get; set; } = new List<int>();
    }
}
