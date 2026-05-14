using EmployeeLeaveManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeLeaveManagement.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(int roleId, string moduleName, string permissionName);
    Task<bool> HasPermissionAsync(string roleName, string moduleName, string permissionName);
}

public class PermissionService : IPermissionService
{
    private readonly EmployeeLeaveManagementContext _context;

    public PermissionService(EmployeeLeaveManagementContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(int roleId, string moduleName, string permissionName)
    {
        string permissionKey = $"{moduleName}.{permissionName}";
        
        var hasPermission = await _context.RolePermissions
            .Include(rp => rp.Permission)
            .AnyAsync(rp => rp.RoleId == roleId 
                            && rp.Permission.PermissionKey == permissionKey 
                            && rp.IsAllowed);
                            
        return hasPermission;
    }

    public async Task<bool> HasPermissionAsync(string roleName, string moduleName, string permissionName)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        if (role == null) return false;
        
        return await HasPermissionAsync(role.RoleId, moduleName, permissionName);
    }
}
