using System;
using System.Collections.Generic;
using Final_year_Project.Device.Domain.EnumTypes;

namespace Final_year_Project.Device.Domain.Entities;

public partial class Lane
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public LaneType Type { get; set; } 

    public int? ReverseLane { get; set; }

    public int ComputerId { get; set; }

    public LaneAutoOpenBarrier AutoOpenBarrier { get; set; }

    public bool Loop { get; set; }

    public bool DisplayLed { get; set; }

    public bool Status { get; set; }

    public bool Deleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Computer Computer { get; set; } = null!;

    public virtual ICollection<LaneCamera> LaneCameras { get; set; } = new List<LaneCamera>();

    public virtual ICollection<LaneControlUnit> LaneControlUnits { get; set; } = new List<LaneControlUnit>();
}
