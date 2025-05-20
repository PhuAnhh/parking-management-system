using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class Camera
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string IpAddress { get; set; } = null!;

    public string? Resolution { get; set; }

    public string? Type { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int ComputerId { get; set; }

    public bool Status { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Computer Computer { get; set; } = null!;

    public virtual ICollection<LaneCamera> LaneCameras { get; set; } = new List<LaneCamera>();
}
