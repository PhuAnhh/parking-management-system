using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class EntryLog
{
    public int Id { get; set; }

    public string? PlateNumber { get; set; }

    public int CardId { get; set; }

    public int? CardGroupId { get; set; }

    public int LaneId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime EntryTime { get; set; }

    public string? ImageUrl { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Card Card { get; set; } = null!;

    public virtual CardGroup? CardGroup { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<ExitLog> ExitLogs { get; set; } = new List<ExitLog>();

    public virtual Lane Lane { get; set; } = null!;
}
