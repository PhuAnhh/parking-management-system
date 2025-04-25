using System.Text.Json.Serialization;
using Final_year_Project.Device.Domain.EnumTypes;

namespace Final_year_Project.Device.Application.Models
{
    public class LedDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public int ComputerId { get; set; }

        public string? Comport { get; set; }

        public int Baudrate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LedType Type { get; set; } 

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateLedDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public int ComputerId { get; set; }

        public string? Comport { get; set; }

        public int Baudrate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LedType Type { get; set; }

        public bool Status { get; set; }
    }

    public class UpdateLedDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public int ComputerId { get; set; }

        public string? Comport { get; set; }

        public int Baudrate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LedType Type { get; set; } 

        public bool Status { get; set; }
    }
}
