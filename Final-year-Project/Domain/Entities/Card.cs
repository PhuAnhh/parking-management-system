using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class Card
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int CardGroupId { get; set; }

    public int? CustomerId { get; set; }

    public string? Note { get; set; }

    public string Status { get; set; } = null!;

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CardGroup CardGroup { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();

    public virtual ICollection<ExitLog> ExitLogs { get; set; } = new List<ExitLog>();
}
