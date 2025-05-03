using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class CustomerGroup
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
