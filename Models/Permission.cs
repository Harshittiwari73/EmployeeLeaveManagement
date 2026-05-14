using System;
using System.Collections.Generic;

namespace EmployeeLeaveManagement.Models;

public partial class Permission
{
    public int PermissionId { get; set; }
    public string ModuleName { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
    public string PermissionKey { get; set; } = "";
    public string Module { get; set; } = "";
    public string? Description { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
