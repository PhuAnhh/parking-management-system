using System;
using System.Collections.Generic;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Domain.Entities;

public partial class ControlUnit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Comport { get; set; }

    public int? Baudrate { get; set; }

    public ControlUnitType Type { get; set; }

    public ControlUnitConnectionProtocolType ConnectionProtocol { get; set; }

    public int ComputerId { get; set; }

    public bool Status { get; set; }

    public bool Deleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Computer Computer { get; set; } = null!;

    public virtual ICollection<LaneControlUnit> LaneControlUnits { get; set; } = new List<LaneControlUnit>();
}
