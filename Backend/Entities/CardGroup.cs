using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class CardGroup
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string VehicleType { get; set; } = null!;

    public int? FreeMinutes { get; set; }

    public int? FirstBlockMinutes { get; set; }

    public decimal? FirstBlockPrice { get; set; }

    public int? NextBlockMinutes { get; set; }

    public decimal? NextBlockPrice { get; set; }

    public bool Status { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CardGroupLane> CardGroupLanes { get; set; } = new List<CardGroupLane>();

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();

    public virtual ICollection<ExitLog> ExitLogs { get; set; } = new List<ExitLog>();

    public virtual ICollection<RevenueReport> RevenueReports { get; set; } = new List<RevenueReport>();
}
