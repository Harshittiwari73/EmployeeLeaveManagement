using System.Collections.Generic;

namespace EmployeeLeaveManagement.Models;

public class RoleViewModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<ModulePermissionViewModel> Modules { get; set; } = new();
}
public class ModulePermissionViewModel
{
    public string ModuleName { get; set; } = string.Empty;
    public List<PermissionItemViewModel> Permissions { get; set; } = new();
}
public class PermissionItemViewModel
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
}
