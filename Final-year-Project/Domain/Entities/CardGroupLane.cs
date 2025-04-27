using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class CardGroupLane
{
    public int Id { get; set; }

    public int CardGroupId { get; set; }

    public int LaneId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CardGroup CardGroup { get; set; } = null!;

    public virtual Lane Lane { get; set; } = null!;
}
