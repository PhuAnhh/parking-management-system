using System.Text.Json.Serialization;
using Final_year_Project.Device.Domain.EnumTypes;

namespace Final_year_Project.Device.Application.Models
{
    public class ControlUnitDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Comport { get; set; }

        public int? Baudrate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ControlUnitType Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ControlUnitConnectionProtocolType ConnectionProtocol { get; set; }

        public int ComputerId { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CreateControlUnitDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Comport { get; set; }

        public int? Baudrate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ControlUnitType Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ControlUnitConnectionProtocolType ConnectionProtocol { get; set; } 

        public int ComputerId { get; set; }

        public bool Status { get; set; }
    }

    public class UpdateControlUnitDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Comport { get; set; }

        public int? Baudrate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ControlUnitType Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ControlUnitConnectionProtocolType ConnectionProtocol { get; set; } 

        public int ComputerId { get; set; }

        public bool Status { get; set; }
    }
}
