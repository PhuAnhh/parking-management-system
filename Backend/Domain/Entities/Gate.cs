using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class Gate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool Status { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Computer> Computers { get; set; } = new List<Computer>();
}
