using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Endpoint { get; set; } = null!;

    public string Method { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
