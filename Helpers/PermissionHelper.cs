using System.Security.Claims;
using EmployeeLeaveManagement.Services;

namespace EmployeeLeaveManagement.Helpers
{
    public static class PermissionHelper
    {
        public static async Task<bool> HasPermission(ClaimsPrincipal user, string moduleName, string permissionName, HttpContext context)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim))
            {
                return false;
            }

            var permissionService = context.RequestServices.GetService<IPermissionService>();
            if (permissionService == null)
            {
                return false;
            }

            return await permissionService.HasPermissionAsync(roleClaim, moduleName, permissionName);
        }
    }
}
