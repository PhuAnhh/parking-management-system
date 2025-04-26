using System;
using System.Collections.Generic;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Domain.Entities;

public partial class Camera
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string IpAddress { get; set; } = null!;

    public string? Resolution { get; set; }

    public CameraType Type { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int ComputerId { get; set; }

    public bool Status { get; set; }

    public bool Deleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Computer Computer { get; set; } = null!;

    public virtual ICollection<LaneCamera> LaneCameras { get; set; } = new List<LaneCamera>();
}
