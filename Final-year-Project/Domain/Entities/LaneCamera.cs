using System;
using System.Collections.Generic;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Domain.Entities;
public partial class LaneCamera
{
    public int Id { get; set; }

    public int LaneId { get; set; }

    public int CameraId { get; set; }

    public LaneCameraPurpose Purpose { get; set; } 

    public int DisplayPosition { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Camera Camera { get; set; } = null!;

    public virtual Lane Lane { get; set; } = null!;
}
