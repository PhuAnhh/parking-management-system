using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class ExitLog
{
    public int Id { get; set; }

    public int EntryLogId { get; set; }

    public string? ExitPlateNumber { get; set; }

    public int CardId { get; set; }

    public int CardGroupId { get; set; }

    public int EntryLaneId { get; set; }

    public int ExitLaneId { get; set; }

    public DateTime EntryTime { get; set; }

    public DateTime ExitTime { get; set; }

    public TimeOnly? TotalDuration { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Note { get; set; }

    public string? ImageUrl { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Card Card { get; set; } = null!;

    public virtual CardGroup CardGroup { get; set; } = null!;

    public virtual Lane EntryLane { get; set; } = null!;

    public virtual EntryLog EntryLog { get; set; } = null!;

    public virtual Lane ExitLane { get; set; } = null!;
}
