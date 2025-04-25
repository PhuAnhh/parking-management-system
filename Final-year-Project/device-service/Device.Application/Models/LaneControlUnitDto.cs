using Final_year_Project.Device.Domain.Entities;

namespace Final_year_Project.Device.Application.Models
{
    public class LaneControlUnitDto
    {
        public int Id { get; set; }

        public int LaneId { get; set; }

        public int ControlUnitId { get; set; }

        public string? Reader { get; set; }

        public string? Input { get; set; }

        public string? Barrier { get; set; }

        public string? Alarm { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CreateLaneControlUnitDto
    {
        public int ControlUnitId { get; set; }

        public string? Reader { get; set; }

        public string? Input { get; set; }

        public string? Barrier { get; set; }

        public string? Alarm { get; set; }
    }

    public class UpdateLaneControlUnitDto
    {
        public int ControlUnitId { get; set; }

        public string? Reader { get; set; }

        public string? Input { get; set; }

        public string? Barrier { get; set; }

        public string? Alarm { get; set; }
    }
}
