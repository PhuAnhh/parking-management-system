using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class LaneControlUnit
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

    public virtual ControlUnit ControlUnit { get; set; } = null!;

    public virtual Lane Lane { get; set; } = null!;
}
