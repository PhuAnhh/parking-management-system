using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class Lane
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? ReverseLane { get; set; }

    public int ComputerId { get; set; }

    public string AutoOpenBarrier { get; set; } = null!;

    public bool Loop { get; set; }

    public bool DisplayLed { get; set; }

    public bool Status { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CardGroupLane> CardGroupLanes { get; set; } = new List<CardGroupLane>();

    public virtual Computer Computer { get; set; } = null!;

    public virtual ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();

    public virtual ICollection<ExitLog> ExitLogEntryLanes { get; set; } = new List<ExitLog>();

    public virtual ICollection<ExitLog> ExitLogExitLanes { get; set; } = new List<ExitLog>();

    public virtual ICollection<LaneCamera> LaneCameras { get; set; } = new List<LaneCamera>();

    public virtual ICollection<LaneControlUnit> LaneControlUnits { get; set; } = new List<LaneControlUnit>();

    public virtual ICollection<WarningEvent> WarningEvents { get; set; } = new List<WarningEvent>();
}
