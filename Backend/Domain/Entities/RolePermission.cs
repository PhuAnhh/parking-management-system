using System;
using System.Collections.Generic;

namespace Final_year_Project.Domain.Entities;

public partial class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Permission Permission { get; set; } = null!;

    public Role Role { get; set; } = null!;
}
