using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class LaneCamera
{
    public int Id { get; set; }

    public int LaneId { get; set; }

    public int CameraId { get; set; }

    public string Purpose { get; set; } = null!;

    public int DisplayPosition { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Camera Camera { get; set; } = null!;

    public virtual Lane Lane { get; set; } = null!;
}
