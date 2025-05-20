using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? CustomerGroupId { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual CustomerGroup? CustomerGroup { get; set; }

    public virtual ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();
}
