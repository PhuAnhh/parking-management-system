using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Domain.EnumTypes;

namespace Final_year_Project.Device.Application.Models
{
    public class LaneCameraDto
    {
        public int Id { get; set; }

        public int LaneId { get; set; }

        public int CameraId { get; set; }

        public LaneCameraPurpose Purpose { get; set; } 

        public int DisplayPosition { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }

    public class CreateLaneCameraDto
    {
        public int CameraId { get; set; }

        public LaneCameraPurpose Purpose { get; set; }

        public int DisplayPosition { get; set; }
    }

    public class UpdateLaneCameraDto
    {
        public int CameraId { get; set; }

        public LaneCameraPurpose Purpose { get; set; }

        public int DisplayPosition { get; set; }
    }
}
