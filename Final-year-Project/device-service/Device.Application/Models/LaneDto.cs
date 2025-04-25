using System.Text.Json.Serialization;
using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Domain.EnumTypes;

namespace Final_year_Project.Device.Application.Models
{
    public class LaneDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LaneType Type { get; set; }

        public int? ReverseLane { get; set; }

        public int ComputerId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LaneAutoOpenBarrier AutoOpenBarrier { get; set; }

        public bool Loop { get; set; }

        public bool DisplayLed { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<LaneCameraDto> LaneCameras { get; set; } = new List<LaneCameraDto>();

        public List<LaneControlUnitDto> LaneControlUnits { get; set; } = new List<LaneControlUnitDto>();
    }

    public class CreateLaneDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LaneType Type { get; set; }

        public int? ReverseLane { get; set; }

        public int ComputerId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LaneAutoOpenBarrier AutoOpenBarrier { get; set; } 

        public bool Loop { get; set; }

        public bool DisplayLed { get; set; }

        public bool Status { get; set; }

        public List<CreateLaneCameraDto> LaneCameras { get; set; } = new List<CreateLaneCameraDto>();

        public List<CreateLaneControlUnitDto> LaneControlUnits { get; set; } = new List<CreateLaneControlUnitDto>();
    }

    public class UpdateLaneDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LaneType Type { get; set; }

        public int? ReverseLane { get; set; }

        public int ComputerId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LaneAutoOpenBarrier AutoOpenBarrier { get; set; } 

        public bool Loop { get; set; }

        public bool DisplayLed { get; set; }

        public bool Status { get; set; }

        public List<UpdateLaneCameraDto> LaneCameras { get; set; } = new List<UpdateLaneCameraDto>();

        public List<UpdateLaneControlUnitDto > LaneControlUnits { get; set;} = new List<UpdateLaneControlUnitDto>();
    }
}
