using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class WarningEvent
{
    public int Id { get; set; }

    public string? PlateNumber { get; set; }

    public int LaneId { get; set; }

    public string? WarningType { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Lane Lane { get; set; } = null!;
}
