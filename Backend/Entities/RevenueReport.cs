using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class RevenueReport
{
    public int Id { get; set; }

    public int CardGroupId { get; set; }

    public int ExitCount { get; set; }

    public decimal Revenue { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CardGroup CardGroup { get; set; } = null!;
}
