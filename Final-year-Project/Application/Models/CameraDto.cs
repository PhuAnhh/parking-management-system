using System.Text.Json.Serialization;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Application.Models
{
    public class CameraDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public string? Resolution { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CameraType Type { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int ComputerId { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCameraDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public string? Resolution { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CameraType Type { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int ComputerId { get; set; }

        public bool Status { get; set; }
    }

    public class UpdateCameraDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public string? Resolution { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CameraType Type { get; set; } 

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int ComputerId { get; set; }

        public bool Status { get; set; }
    }
}
