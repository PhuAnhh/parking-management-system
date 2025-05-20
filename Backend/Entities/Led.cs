using System;
using System.Collections.Generic;

namespace Final_year_Project.Entities;

public partial class Led
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int ComputerId { get; set; }

    public string? Comport { get; set; }

    public int Baudrate { get; set; }

    public string Type { get; set; } = null!;

    public bool Status { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Computer Computer { get; set; } = null!;
}
